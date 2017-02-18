namespace SerebiiParser.Core

open FSharp.Data
open System
open System.Globalization

module AbilityParser =

    let private baseUrl = "http://serebii.net/abilitydex/"

    let private selects =
        let results = HtmlDocument.Load(baseUrl)
        
        results.Descendants("select")
            |> Seq.filter (fun s ->
                s.TryGetAttribute("name")
                    |> Option.map (fun n -> n.Value())
                        = Some "SelectURL")

    let abilityNames =
        selects
            |> Seq.collect (fun s ->
                s.Descendants("option"))
            |> Seq.map (fun i ->
                i.Attribute("value").Value())
            |> Seq.filter (fun i ->
                i <> "index.shtml")
            |> Seq.map (fun i ->
                i.Replace("/abilitydex/", "").Replace(".shtml", ""))
            |> Seq.sort

    let abilityUrl name =
        sprintf "%s%s%s" baseUrl name ".shtml"

    let parseAbility name =
        let url = sprintf "%s%s.shtml" baseUrl name
        let results = HtmlDocument.Load(url)
        let dexTables = 
            results.Descendants("table") 
                |> Seq.filter (fun t -> 
                    t.TryGetAttribute("class")
                        |> Option.map (fun c -> c.Value())
                            = Some "dextable")

        let abilityTable =
            dexTables |> Seq.item 1

        let abilityRows =
            abilityTable.Descendants("tr", false)

        let description =
            let row = abilityRows |> Seq.item 1
            let cell = row.Descendants("td", false) |> Seq.item 0
            cell.InnerText()

        let newName = description.ToLowerInvariant().Replace(" ", "_").Replace("'", "")

        let effect =
            let index = abilityRows |> Seq.findIndex (fun i -> i.InnerText() = "Game's Text:")
            let row = abilityRows |> Seq.item (index + 1)
            let cell = row.Descendants("td", false) |> Seq.item 0
            cell.InnerText()

        {name = newName;
        description = description;
        effect = effect}
