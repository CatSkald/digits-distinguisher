module Calculation

open FSharp.Data
open System

[<Literal>] let size = 28
let recognizedImagesCsv = CsvFile.Load("./data/trainingsample.csv").Cache();
let imagesToRecognizeCsv = CsvFile.Load("./data/validationsample.csv").Cache();

type Image = {
    Label: string
    Pixels: int[]
}

type DistinguishedResult = {
    Expected: string
    Actual: string
    Distance: int
    Matches: bool
}

let loadImagesFromCsv (csvRows : seq<CsvRow>) : Image[] =
    csvRows
    |> Seq.toArray
    |> Array.map (fun row ->
    {
        Label = row.Columns.[0];
        Pixels = row.Columns.[1..] |> Array.map int
    })

let loadImagesToRecognize : Image[] = 
    imagesToRecognizeCsv.Rows
    |> loadImagesFromCsv
    
let loadRecognizedImages : Image[] = 
    recognizedImagesCsv.Rows
    |> loadImagesFromCsv

let getManhattanDistance image1 image2 : int =
    Array.map2 (fun pixelA pixelB -> abs (pixelA - pixelB)) image1.Pixels image2.Pixels
    |> Array.sum
    
let getEucledianDistance image1 image2 : int =
    Array.map2 (fun pixelA pixelB -> (pixelA - pixelB) |> (fun x -> x*x)) image1.Pixels image2.Pixels
    |> Array.sum
    |> float
    |> Math.Sqrt
    |> int

let classifierEucledian image : DistinguishedResult =
    loadRecognizedImages
    |> Array.map (fun sample -> sample.Label, getEucledianDistance image sample)
    |> Array.minBy snd
    |> fun (actual, distance) -> {
        Expected = image.Label;
        Actual = actual;
        Distance = distance;
        Matches = image.Label = actual
    }

let classifierManhattan image : DistinguishedResult =
    loadRecognizedImages
    |> Array.map (fun sample -> sample.Label, getManhattanDistance image sample)
    |> Array.minBy snd
    |> fun (actual, distance) -> {
        Expected = image.Label;
        Actual = actual;
        Distance = distance;
        Matches = image.Label = actual
    }

let execute =
    loadImagesToRecognize
    |> Array.map classifierEucledian
    |> Array.filter (fun result -> not result.Matches)
    |> Array.length
