namespace SerebiiParser.CodeGenerators.Ruby

open SerebiiParser.Core
open SerebiiParser.Core.AbilityParser
open System.IO
open System.Collections.Generic
open System.Threading.Tasks
open System.Text
open SerebiiParser.CodeGenerators
open SerebiiParser.CodeGenerators.Collections

module AbilityCodeGenerator =
    let generateDbSeedsFromSerebii dir =
        let errors = ErrorLog()
        let abilities = AbilityList()

        let storeAbility a =
            printfn "Parsed:\n%A\n" a
            abilities.Add a

        Parallel.ForEach(abilityNames, (fun name ->
            try
                parseAbility name |> storeAbility
            with
                | _ as ex -> abilityUrl name |> errors.Add ex.Message)) |> ignore

        let saveAbilities (list : IEnumerable<Ability>) =
            let sorted = list |> List.ofSeq |> List.sortBy (fun a -> a.name)
            let grouped = sorted |> List.groupBy (fun a -> a.name.[0])
        
            for (key, items) in grouped do
                let fname = Path.Combine(dir, sprintf "%c_abilities.rb" key)
                let sb = StringBuilder()
                use file = File.CreateText fname
                for a in items do
                    let code = (sprintf @"Ability.create!(
  name: '%s',
  description: '%s',
  effect: '%s'
)

" a.name a.description a.effect)
                    code |> sb.Append |> ignore
                    let text = sb.ToString()
                    text.Substring(0, text.Length - 1) |> file.Write
            
        if abilities.Count > 0
        then
            printfn "Saving Abilities..."
            saveAbilities abilities
        else
            printfn "No Ability parsed."
    
        if errors.Count > 0
        then
            printfn "Saving errors..."
            Path.Combine(dir, "AbilityParsingErrors.log") |> errors.Save
        else
            printfn "No errors found. Perfect parsing!"