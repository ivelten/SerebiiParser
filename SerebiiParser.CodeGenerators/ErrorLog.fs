namespace SerebiiParser.CodeGenerators

open System.Collections.Generic
open System.Text
open System.IO

type ErrorLog() =
    let dict = Dictionary<string, List<string>>()
    member private x.AddNewError msg url =
        let list = List<string>()
        list.Add url
        dict.Add(msg, list)
    member private x.AddError msg url =
        dict.[msg].Add url
    member x.Add msg url =
        if dict.ContainsKey msg
        then x.AddError msg url
        else x.AddNewError msg url
    member x.Item
        with get i = dict.[i]
        and set i v = dict.[i] <- v
    member x.Count = dict.Count
    member x.Save path =
        use file = File.CreateText path
        for entry in dict do
            sprintf "Error: %s\r\nPages:\r\n" entry.Key |> file.Write
            for uri in entry.Value do 
                sprintf "%s\r\n" uri |> file.Write
            file.Write "\r\n\r\n"