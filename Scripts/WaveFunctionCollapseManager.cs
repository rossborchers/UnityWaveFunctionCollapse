
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityWFC;

public class WaveFunctionCollapseManager : MonoBehaviour
{
    public async Task GenerateAsync()
    {
        System.Random random = new System.Random();
        XDocument xdoc = XDocument.Load(Helper.GetStreamingAssetsPath("Samples.xml"));

        foreach (XElement xElement in xdoc.Root.Elements("overlapping", "simpletiled"))
        {
            Model model;
            string nameAttrib = xElement.Get<string>("name");
            bool isOverlapping = xElement.Name == "overlapping";
            int size = xElement.Get("size", isOverlapping ? 48 : 24);
            int width = xElement.Get("width", size);
            int height = xElement.Get("height", size);
            bool periodic = xElement.Get("periodic", false);
            string heuristicString = xElement.Get<string>("heuristic");
            var heuristic = Model.Heuristic.Scanline;
            if (heuristicString != "Scanline")
            {
                heuristic = heuristicString == "MRV" ? Model.Heuristic.MRV : Model.Heuristic.Entropy;
            }

            if (isOverlapping)
            {
                int N = xElement.Get("N", 3);
                bool periodicInput = xElement.Get("periodicInput", true);
                int symmetry = xElement.Get("symmetry", 8);
                int ground = xElement.Get("ground", 0);
                
                Debug.Log($"=== OVERLAPPING: Name: {nameAttrib},  Order: {N}, Width {width}, Height {height}, Periodic Input: {periodicInput}, Periodic: {periodic}, Ground Layer: {ground}, Heuristic: {heuristic} ===");

                model = new OverlappingModel();
                await ((OverlappingModel)model).OpenModel(nameAttrib, N, width, height, periodicInput, periodic, symmetry, ground, heuristic);
            }
            else
            {
                string subset = xElement.Get<string>("subset");
                bool blackBackground = xElement.Get("blackBackground", false);
                
                Debug.Log($"=== TILED: Name: {nameAttrib}, Subset: {subset}, Width {width}, Height {height}, Periodic: {periodic}, Black background: {blackBackground}, Heuristic: {heuristic} ===");

                model = new SimpleTiledModel();
                await ((SimpleTiledModel)model).OpenModel(nameAttrib, subset, width, height, periodic, blackBackground, heuristic);
            }

            for (int i = 0; i < xElement.Get("screenshots", 2); i++)
            {
                for (int k = 0; k < 10; k++)
                {
                    Debug.Log($" === {nameAttrib} Over === ");
                    int seed = random.Next();
                    bool success = model.Run(seed, xElement.Get("limit", -1));
                    if (success)
                    {
                        Debug.Log("DONE");
                        string path = Helper.GetStreamingAssetsPath($"{nameAttrib} {seed}.png");
                        Debug.Log(path);
                        await model.GenerateImage().Save(path);
                        
                        if (model is SimpleTiledModel stModel && xElement.Get("textOutput", false))
                            //TODO: convert to unityweb request
                            System.IO.File.WriteAllText(Helper.GetStreamingAssetsPath($"{nameAttrib} {seed}.txt"),stModel.TextOutput());
                        break;
                    }
                    else  Debug.Log("CONTRADICTION");
                }
            }
        }
        
        Debug.Log("TOTALLY COMPLETE");
    }
}
