[<ReflectedDefinition>]
module FSharpIDE

open FunScript
open FunScript.TypeScript
open FunScript.TypeScript.fs
open FunScript.TypeScript.child_process
open FunScript.TypeScript.AtomCore
open FunScript.TypeScript.text_buffer
open FunScript.TypeScript.path

open Atom
open Atom.FSharp

type FSharpIDE() =
    let subscriptions = ResizeArray()

    member x.provide () =
        AutocompleteProvider.create ()

    member x.provideErrors () =
        ErrorLinterProvider.create ()

    member x.getSuggestion(options : AutocompleteProvider.GetSuggestionOptions) =
        AutocompleteProvider.getSuggestion options

    member x.consumeYeomanEnvironment (gen : YeomanHandler.generator) =
        YeomanHandler.activate gen

    member x.activate(state:obj) =

        let show = Globals.atom.config.get("atom-fsharp.ShowQuickInfoPanel") |> unbox<bool>
        let highlight = Globals.atom.config.get("atom-fsharp.ShowUseHighlights") |> unbox<bool>
        let debug = Globals.atom.config.get("atom-fsharp.DeveloperMode") |> unbox<bool>

        LanguageService.start ()
        Parser.activate ()
        TooltipHandler.activate ()
        if show then ToolbarHandler.activate()
        FindDeclaration.activate ()
        FAKE.activate ()
        Interactive.activate ()
        if highlight then HighlightUse.activate ()
        AddFileHandler.activate ()
        FormatHandler.activate ()
        if debug then DeveloperMode.activate ()

        let s = Globals.atom.config.onDidChange ("atom-fsharp.ShowQuickInfoPanel",
                    (fun n -> if n.newValue then ToolbarHandler.activate() else ToolbarHandler.deactivate()  ) |> unbox<Function>)
        let s2 = Globals.atom.config.onDidChange ("atom-fsharp.ShowUseHighlights",
                    (fun n -> if n.newValue then HighlightUse.activate() else HighlightUse.deactivate()  ) |> unbox<Function>)
        let s3 = Globals.atom.config.onDidChange ("atom-fsharp.DeveloperMode",
                    (fun n -> if n.newValue then DeveloperMode.activate() else DeveloperMode.deactivate()  ) |> unbox<Function>)


        subscriptions.Add s
        subscriptions.Add s2
        subscriptions.Add s3

        ()

    member x.deactivate() =
        subscriptions |> Seq.iter(fun n -> n.dispose())
        subscriptions.Clear()
        let show = Globals.atom.config.get("atom-fsharp.ShowQuickInfoPanel") |> unbox<bool>

        Parser.deactivate ()
        TooltipHandler.deactivate ()
        if show then ToolbarHandler.deactivate()
        FindDeclaration.deactivate ()
        FAKE.deactivate ()
        Interactive.deactivate ()
        HighlightUse.deactivate ()
        LanguageService.stop ()
        DeveloperMode.activate ()
        ()
