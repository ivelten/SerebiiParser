namespace SerebiiParser.Core

open FSharp.Data
open System
open System.Globalization

module PokemonParser =

    let threeDigitsNumber n = n.ToString().PadLeft(3, '0')
    let pokemonUrl n = threeDigitsNumber n |> sprintf "http://serebii.net/pokedex-sm/%s.shtml"
    
    let private culture = CultureInfo.GetCultureInfo("en-US")

    let parsePokemon n =
        let url = pokemonUrl n
        let results = HtmlDocument.Load(url)

        let splitUnits(s:string) = s.Split([|" "; "/"; "\r\n"|], StringSplitOptions.RemoveEmptyEntries)

        let parseNumber(s:string) =
            let split = splitUnits s

            if split.Length = 1
            then Int32.Parse(s, NumberStyles.AllowThousands, culture)
            else Int32.Parse(split.[0].Replace(" (Core)", ""), NumberStyles.AllowThousands, culture)

        let parseHeight(s:string) =
            let split = splitUnits s
        
            if split.Length = 4
            then Decimal.Parse(split.[3].Replace("m", ""), culture)
            else Decimal.Parse(split.[1].Replace("m", ""), culture)

        let parseWeight(s:string) =
            let split = splitUnits s
        
            if split.Length = 4
            then Decimal.Parse(split.[2].Replace("kg", ""), culture)
            else Decimal.Parse(split.[1].Replace("kg", ""), culture)

        let parseExperienceGrowth(s:string) =
            parseNumber(s.Split([|" "|], StringSplitOptions.None).[0])

        let dexTables = 
            results.Descendants("table") 
                |> Seq.filter (fun t -> 
                    t.TryGetAttribute("class")
                        |> Option.map (fun c -> c.Value())
                            = Some "dextable")
        
        let statsTable = 
            dexTables
                |> Seq.filter (fun i -> 
                    i.Descendants("tr", false)
                        |> Seq.exists (fun d -> d.InnerText() = "Stats")) 
                |> Seq.exactlyOne
       
        let statsRow = 
            statsTable.Descendants("tr", false)
                |> Seq.item 2

        let stats = 
            statsRow.Descendants("td", false) 
                |> Seq.skip 1 
                |> Seq.map (fun i -> 
                    i.InnerText() |> parseNumber)

        let hp = stats |> Seq.item 0
        let attack = stats |> Seq.item 1
        let defense = stats |> Seq.item 2
        let spAttack = stats |> Seq.item 3
        let spDefense = stats |> Seq.item 4
        let speed = stats |> Seq.item 5

        let infoTable =
            dexTables
                |> Seq.filter (fun i ->
                        i.Descendants("tr", false)
                            |> Seq.exists (fun d -> d.InnerText() = "PictureNameOther NamesNo.Gender RatioType"))
                |> Seq.item 0

        let getRow index (table:HtmlNode) = 
            let rows = table.Descendants("tr", false)
            rows |> Seq.item index

        let infoRow1 = infoTable |> getRow 1
        let infoRow2 = infoTable |> getRow 3

        let getCell index (tr:HtmlNode) =
            let cells = tr.Descendants("td", false)
            cells |> Seq.item index

        let getCellValue index (tr:HtmlNode) = 
            let cell = getCell index tr
            cell.InnerText()

        let parseMaleGenderRatio(tr:HtmlNode) = 
            let innerText = tr.InnerText()
            if innerText.Contains "Male ♂:" && innerText.Contains "Female ♀:" 
            then
                let cell = tr |> getCell 4
                let innerTable = cell.Descendants("table") |> Seq.item 0
                let innerRow = innerTable.Descendants("tr", false) |> Seq.item 0
                let innerCell = innerRow.Descendants("td", false) |> Seq.item 1
                Some (Decimal.Parse(innerCell.InnerText().Replace("%", ""), NumberStyles.AllowDecimalPoint, culture))
            else None

        let name = infoRow1 |> getCellValue 1
        let classification = (infoRow2 |> getCellValue 0).Replace(" Pokémon", "")
        let height = infoRow2 |> getCellValue 1 |> parseHeight
        let weight = infoRow2 |> getCellValue 2 |> parseWeight
        let captureRate = infoRow2 |> getCellValue 3 |> parseNumber
        let baseEggSteps = infoRow2 |> getCellValue 4 |> parseNumber
        let maleGenderRatio = infoRow1 |> parseMaleGenderRatio

        let parsePokemonType(td:HtmlNode) =
            let types = td.Descendants("a", false) |> Seq.map (fun i -> i.AttributeValue "href")

            let ptype s =
                match s with
                    | "/pokedex-sm/bug.shtml" -> PokemonType.Bug
                    | "/pokedex-sm/dark.shtml" -> PokemonType.Dark
                    | "/pokedex-sm/dragon.shtml" -> PokemonType.Dragon
                    | "/pokedex-sm/electric.shtml" -> PokemonType.Electric
                    | "/pokedex-sm/fairy.shtml" -> PokemonType.Fairy
                    | "/pokedex-sm/fighting.shtml" -> PokemonType.Fighting
                    | "/pokedex-sm/fire.shtml" -> PokemonType.Fire
                    | "/pokedex-sm/flying.shtml" -> PokemonType.Flying
                    | "/pokedex-sm/ghost.shtml" -> PokemonType.Ghost
                    | "/pokedex-sm/grass.shtml" -> PokemonType.Grass
                    | "/pokedex-sm/ground.shtml" -> PokemonType.Ground
                    | "/pokedex-sm/ice.shtml" -> PokemonType.Ice
                    | "/pokedex-sm/normal.shtml" -> PokemonType.Normal
                    | "/pokedex-sm/poison.shtml" -> PokemonType.Poison
                    | "/pokedex-sm/psychic.shtml" -> PokemonType.Psychic
                    | "/pokedex-sm/rock.shtml" -> PokemonType.Rock
                    | "/pokedex-sm/steel.shtml" -> PokemonType.Steel
                    | "/pokedex-sm/water.shtml" -> PokemonType.Water
                    | _ -> failwith "Unrecognized type."

            let type1 = types |> Seq.item 0 |> ptype
            let type2 = 
                if Seq.length types = 2
                then Some (types |> Seq.item 1 |> ptype)
                else None

            (type1, type2)

        let (type1, type2) = infoRow1 |> getCell 5 |> parsePokemonType

        let extraInfoTable =
            dexTables
                |> Seq.filter (fun i ->
                    i.Descendants("tr", false)
                        |> Seq.exists (fun d -> d.InnerText() = "Experience GrowthBase HappinessEffort Values EarnedS.O.S. Calling"))
                |> Seq.item 0

        let infoRow3 = extraInfoTable |> getRow 3

        let experienceGrowth = infoRow3 |> getCellValue 0 |> parseExperienceGrowth
        let baseHappiness = infoRow3 |> getCellValue 1 |> parseNumber

        let parsePokemonAbilities (td:HtmlNode) = 
            let anchors = td.Descendants("a", false)
            let count = Seq.length anchors
            
            let hiddenIndex =
                match count with
                    |1 -> None
                    |2 -> Some 1
                    |_ -> Some 2

            let ability1 = (anchors |> Seq.item 0).InnerText().ToLower(culture).Replace(" ", "_")
            let ability2 =
                if hiddenIndex = Some 2
                then Some ((anchors |> Seq.item 1).InnerText().ToLower(culture).Replace(" ", "_"))
                else None

            let hiddenAbility = 
                hiddenIndex
                    |>Option.map (fun i -> (anchors |> Seq.item i).InnerText().ToLower(culture).Replace(" ", "_")) 

            (ability1, ability2, hiddenAbility)

        let infoRow4 = extraInfoTable |> getRow 1
        let (ability1, ability2, hiddenAbility) = parsePokemonAbilities infoRow4

        {number = n
         name = name
         classification = classification
         type1 = type1
         type2 = type2
         height = height
         weight = weight
         captureRate = captureRate
         baseEggSteps = baseEggSteps
         maleGenderRatio = maleGenderRatio
         experienceGrowth = experienceGrowth
         baseHappiness = baseHappiness
         hp = hp
         attack = attack
         defense = defense
         spAttack = spAttack
         spDefense = spDefense
         speed = speed
         ability1 = ability1
         ability2 = ability2
         hiddenAbility = hiddenAbility}