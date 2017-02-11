namespace SerebiiParser.Core

open FSharp.Data
open System
open System.Globalization

module AbilityParser =
    let baseUrl = "http://serebii.net/abilitydex/"
    let results = HtmlDocument.Load(baseUrl)

    let abilityCombos =
        results.Descendants("select")
            |> Seq.filter (fun s ->
                s.TryGetAttribute("name")
                    |> Option.map (fun n -> n.Value())
                        = Some "SelectURL")

    let abilitiyUrls =
        abilityCombos
            |> Seq.collect (fun c ->
                c.Descendants("option"))
            |> Seq.map (fun i ->
                i.Attribute("value").Value())
            |> Seq.filter (fun i ->
                i <> "index.shtml")
            |> Seq.map (fun i ->
                i.Replace("/abilitydex/", ""))