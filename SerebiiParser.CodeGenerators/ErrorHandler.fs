namespace SerebiiParser.CodeGenerators

open System.Collections.Generic
open System.IO
open SerebiiParser.CodeGenerators.Collections

module ErrorHandler =
    let private addError msg url (list : ErrorDictionary) = list.[msg].Add url
    
    let private createErrorListWithOneError url =
        let list = List<string>()
        list.Add url
        list

    let treatError msg url (list : ErrorDictionary) =
        if list.ContainsKey msg
        then addError msg url list
        else list.Add(msg, createErrorListWithOneError url)

    let saveErrors dir (list : ErrorDictionary) =
        use file = Path.Combine(dir, "Errors.log") |> File.CreateText
        for entry in list do
            sprintf "Error: %s\r\nPages:\r\n" entry.Key |> file.Write
            for uri in entry.Value do 
                sprintf "%s\r\n" uri |> file.Write
            file.Write "\r\n\r\n"