// include Fake lib
#r "paket:
storage: packages

nuget FSharp.Core 4.7.0.0
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.DotNet.MSBuild //"

#if !FAKE
#load "./.fake/build.fsx/intellisense.fsx"
#endif

open Fake.Core
open Fake.DotNet
open Fake.IO.Globbing.Operators
open Fake.IO

let buildDir = "./Build/"

let projects =
    (!! "./src/*.csproj") :> seq<string> |> List.ofSeq

let modName =
    match projects with
    | [ project ] -> (FileInfo.ofPath project).Name
    | _ -> failwith "Please have one and only one project in the src directory"

let installFolder =
    sprintf
        "%s/Colossal Order/Cities_Skylines/Addons/Mods/%s"
        (Environment.environVar "LOCALAPPDATA")
        (Path.changeExtension "" modName)

Target.create "Clean" (fun _ -> Shell.cleanDir buildDir)

Target.create
    "Restore"
    (fun _ ->
        let projects = !! "./src/*.csproj"

        for project in projects do
            DotNet.restore (fun _ -> DotNet.RestoreOptions.Create()) (project))

Target.create
    "Build"
    (fun _ ->
        !! "./src/*.csproj"
        |> MSBuild.runRelease id buildDir "Build"
        |> Trace.logItems "AppBuild-Output: ")

Target.create
    "Install"
    (fun _ ->
        Shell.mkdir (installFolder)
        Shell.copyFile installFolder (Path.combine buildDir (Path.changeExtension ".dll" modName))
        Shell.copyFile installFolder (Path.combine buildDir (Path.changeExtension ".pdb" modName)))

open Fake.Core.TargetOperators

// Dependencies
"Clean" ==> "Build" ==> "Install"

// start build
Target.runOrDefault "Install"
