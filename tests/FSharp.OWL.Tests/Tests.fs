module FSharp.OWL.Tests

open NUnit.Framework
open System
open System.IO
open System.Net
open Providers
open ProviderImplementation.ProvidedTypes
open TypeProviderInstantiation
open Store
open Schema

type Platform = Net40

let (++) a b = Path.Combine(a, b)

type fixture() = 
    
    [<Test>]
    member public x.``Expression tree for Pizza``() = 
        let resolutionFolder = ""
        let outputFolder = __SOURCE_DIRECTORY__ + "/expected"
        printf "write expressions to %s\r\n" outputFolder
        let assemblyName = "FSharp.OWL.dll"
        
        let dump signatureOnly ignoreOutput saveToFileSystem (inst : TypeProviderInstantiation) = 
            inst.Dump resolutionFolder (if saveToFileSystem then outputFolder
                                        else "") (".." ++ "bin" ++ assemblyName) signatureOnly ignoreOutput
            |> Console.WriteLine
        
        let dumpAll inst = dump false false true inst
        
        let args = 
            { Path = __SOURCE_DIRECTORY__ ++ "pizza.xml"
              BaseUri = "http://www.co-ode.org/ontologies/pizza/pizza.owl#Soho"
              NSMap = """base:http://www.co-ode.org/ontologies/pizza/pizza.owl#,
                         wine:http://www.w3.org/TR/2003/PR-owl-guide-20031209/wine#""" }
        dumpAll (TypeProviderInstantiation.FileProvider(args))
    
    [<Test>]
    member public x.``Generate select query``() = 
        let wineStore = loadFile (__SOURCE_DIRECTORY__ ++ "pizza.xml")
        let rs = 
            resultset 
                (QueryType.Select [ Binding.Wildcard ], 
                 Where [ BGP.a "s" (Node.Uri(Uri.Uri "http://www.w3.org/2002/07/owl#NamedIndividual")) ]) wineStore
        printfn "%A" rs

(*
[<Literal>]
let ttl = __SOURCE_DIRECTORY__ + "/pizza.xml"
type foodOntology = LinkedData.Memory<ttl, "http://www.co-ode.org/ontologies/pizza/pizza.owl#Food","""base:http://www.co-ode.org/ontologies/pizza/pizza.owl#"""> 
*)
