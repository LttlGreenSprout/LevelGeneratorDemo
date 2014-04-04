using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LevelGeneratorDemo
{
    class LevelManager
    {
        #region constants
        private const int LEVEL_WIDTH  = 50;
        private const int LEVEL_HEIGHT = 3;
        private const int LAYER_HEIGHT = 3;

        private const double P0 = .25;
        private const double P1 = .25;
        private const double P2 = .25;
        private const double P3 = .25;
        #endregion
        #region local variables
        private List<String> _chunks;
        private int _seed;
        private Random _random;
        private Dictionary<int, List<char[,]>> explicitChunkDict;

        
        

        #endregion
        #region properties
        public int Seed
        {
            get { return _seed; }
            set 
            { 
                _seed = value;
                _random = new Random(value);
            }
        }
        #endregion
        #region constructor
        public LevelManager()
        {
            _chunks = new List<string>();
            _random = new Random(_seed);
            explicitChunkDict = new Dictionary<int, List<char[,]>>();

            char[,] testExplicitChunk0 = new char[4, 8];
            for (int r = 0; r < testExplicitChunk0.GetLength(0); r++)
                for (int c = 0; c < testExplicitChunk0.GetLength(1); c++)
                    testExplicitChunk0[r, c] = '.';
            char[,] testExplicitChunk1 = new char[4, 8];
            for (int r = 0; r < testExplicitChunk1.GetLength(0); r++)
                for (int c = 0; c < testExplicitChunk1.GetLength(1); c++)
                    testExplicitChunk1[r, c] = '=';
            char[,] testExplicitChunk2 = new char[4, 8];
            for (int r = 0; r < testExplicitChunk0.GetLength(0); r++)
                for (int c = 0; c < testExplicitChunk0.GetLength(1); c++)
                    testExplicitChunk2[r, c] = '|';
            char[,] testExplicitChunk3 = new char[4, 8];
            for (int r = 0; r < testExplicitChunk0.GetLength(0); r++)
                for (int c = 0; c < testExplicitChunk0.GetLength(1); c++)
                    testExplicitChunk3[r, c] = '#';
            char[,] testExplicitChunkUnknown = new char[4, 8];
            for (int r = 0; r < testExplicitChunk0.GetLength(0); r++)
                for (int c = 0; c < testExplicitChunk0.GetLength(1); c++)
                    testExplicitChunkUnknown[r, c] = '?';

            explicitChunkDict[0] = new List<char[,]>();
            explicitChunkDict[1] = new List<char[,]>();
            explicitChunkDict[2] = new List<char[,]>();
            explicitChunkDict[3] = new List<char[,]>();
            explicitChunkDict[-1] = new List<char[,]>();


            explicitChunkDict[0].Add(testExplicitChunk0);
            explicitChunkDict[1].Add(testExplicitChunk1);
            explicitChunkDict[2].Add(testExplicitChunk2);
            explicitChunkDict[3].Add(testExplicitChunk3);
            explicitChunkDict[-1].Add(testExplicitChunkUnknown);
        }
        #endregion
        #region methods
            #region loading and saving
        /// <summary>
        /// opens files from .txt files and puts them in the list of chunks
        /// </summary>
        public void LoadChunksFromText()
        {

        }
        /// <summary>
        /// opens files from custom binary files and puts them in the list of chunks
        /// </summary>
        public void LoadChunksFromBinary()
        {

        }
        #endregion //not implemented
            #region private helper methods
            /// <summary>
            /// Returns a random chunk based on the given percetnages
            /// </summary>
            /// <param name="p0">percent to be open</param>
            /// <param name="p1">percent to be tunnel</param>
            /// <param name="p2">percent to be live end</param>
            /// <param name="p3">percent to be dead end</param>
            /// <returns></returns>
            private int RandomChunkType(double p0, double p1, double p2, double p3)
            {
                double n = _random.NextDouble();
                if (n < p0)
                    return 0;
                if (n < p0+p1)
                    return 1;
                if (n < p0+p1+p2)
                    return 2;
                if (n < p0+p1+p2+p3)
                    return 3;
                return -1;
            }
            /// <summary>
            /// Returns a random chunk based on this managers percentages
            /// </summary>
            /// <returns></returns>
            private int RandomChunkType()
            {
                return RandomChunkType(P0, P1, P2, P3);
            }
            #endregion
            #region public helper methods
            public void NextSeed()
            {
                this.Seed = _random.Next();
            }
            #endregion
            #region layer generation
        /// <summary>
        /// Generates a valid layer with at least one guaranteed solution path
        /// using the given seed
        /// </summary>
        /// <param name="seed">Seed to use </param>
        /// <returns>chunk layer matrix</returns>
        public int[,] GenerateIntLayer(int seed)
        {
            //Loads seed into the _random object
            this.Seed = seed;
            //The level to generate and return
            int[,] layer = new int[LAYER_HEIGHT, LEVEL_WIDTH];
            //flag all positions as unvisited
            for (int r = 0; r < layer.GetLength(0); r++)
                for (int c = 0; c < layer.GetLength(1); c++)
                    layer[r, c] = -1;
            //solution path's current position in the layer
            int[] pos = new int[2];
            pos[0] = _random.Next(0, LAYER_HEIGHT); //row position
            pos[1] = 0; //col position
            //move until the position has reached the end of the level
            while (pos[1] < LEVEL_WIDTH)
            {
                //generate the chunk type for the current position
                int chunkType = this.RandomChunkType();
                //store that in the current position
                layer[pos[0], pos[1]] = chunkType;
                double n = _random.NextDouble();
                //change movement depending on current type
                switch (chunkType)
                {
                    case 0:
                        if (n < 0.33 && pos[0] > 0)     //move up
                            pos[0]--;
                        else if (n < 0.66 && pos[0] < LAYER_HEIGHT - 1)//move down
                            pos[0]++;
                        else
                            pos[1]++;   //move right
                        break;
                    case 1:
                        pos[1]++; //move right
                        if (pos[1] != LEVEL_WIDTH && layer[pos[0], pos[1]] == 3) //if I moved right into a dead end, delete the dead end
                            layer[pos[0], pos[1]] = 0;
                        break;
                    case 2:
                        if (n < 0.5 && pos[0] > 0) //move up
                            pos[0]--;
                        else if (pos[0] < 2) //move down
                            pos[0]++;
                        else                //change current and move right
                        {
                            layer[pos[0], pos[1]] = 0;
                            pos[1]++;
                        }
                        break;
                    case 3:
                        if (pos[1] > 0)
                            pos[1]--;
                        break;
                }
            }
            //solution path guaranteed, fill in unvisited squares with random chunks
            //if desired, check for special patterns to insert special chunk patterns
            //ignore path guarantee: it's already been done
            for (int r = 0; r < layer.GetLength(0); r++)
                for (int c = 0; c < layer.GetLength(1); c++)
                    if (layer[r, c] == -1)
                        layer[r, c] = -1;//RandomChunkType(0,0,0,1);
            return layer;
        }
        /// <summary>
        /// Generates a valid chunk layer using the current seed
        /// </summary>
        /// <returns>chunk layer matrix</returns>
        public int[,] GenerateIntLayer()
        {
            return GenerateIntLayer(_seed);
        }
        /// <summary>
        /// Generates a valid chunk layer using the current seed,
        /// then updates the seed
        /// </summary>
        /// <param name="updateSeed">Are you updating the seed?</param>
        /// <returns>chunk layer matrix</returns>
        public int[,] GenerateIntLayer(bool updateSeed)
        {
            int[,] layer = GenerateIntLayer(_seed);
            if (updateSeed) this.Seed = _random.Next();
            return layer;
        }
        /// <summary>
        /// Generates a valid chunk layer using the given seed,
        /// then updates the seed
        /// </summary>
        /// <param name="updateSeed">Are you updating the seed?</param>
        /// <returns>chunk layer matrix</returns>
        public int[,] GenerateIntLayer(int seed, bool updateSeed)
        {
            int[,] layer = GenerateIntLayer(seed);
            if (updateSeed) this.Seed = _random.Next();
            return layer;
        }
        /// <summary>
        /// Generates a valid char layer based on a valid chunk layer by replacing
        /// chunk layer ints with char matrices to build a complete layer
        /// </summary>
        /// <returns></returns>
        public char[,] GenerateCharLayer(int[,] chunkLevel)
        {
            return default(char[,]);
        }
        #endregion
            #region level generation
        /// <summary>
        /// Generate a playable level 
        /// </summary>
        /// <param name="layers"></param>
        public char[,] GenerateLevel(int layers)
        {
            //make sure you don't try to generate a level with zero layers
            if (layers < 1)
                return null;
            //Create a List to store the different chunk layers
            List<int[,]> chunkLayers = new List<int[,]>();
            //Create a List to store the resulting explicit layers
            List<char[,]> explicitLayers = new List<char[,]>();
            //Generate a new chunk layer and store it in the list while updating the seed
            for (int i = 0; i < layers; i++)
                chunkLayers.Add(this.GenerateIntLayer(true));

            //grab the rows and cols of the chunk layer
            int chunkLayerRows = chunkLayers[0].GetLength(0);
            int chunkLayerCols = chunkLayers[0].GetLength(1);
            //grab the rows and cols of the saved explicit chunkFiles
            int explicitChunkRows = explicitChunkDict[0][0].GetLength(0);
            int explicitChunkCols = explicitChunkDict[0][0].GetLength(1);

            //grab each chunk layer and fill in the chunks with loaded explicit chunks
            for (int i = 0; i < chunkLayers.Count; i++)
            {
                //grab the current chunk layer
                int[,] chunkLayer = chunkLayers[i];

                //create an explicit layer with size = [product of chunklayer rows and explicit rows width,product of chunklayer cols and explicit matrix cols]
                char[,] explicitLayer = new char[chunkLayerRows * explicitChunkDict[0][0].GetLength(0), chunkLayerCols * explicitChunkDict[0][0].GetLength(1)];
                //loop through the chunk layer
                for(int r1 = 0; r1 < chunkLayer.GetLength(0); r1 ++)
                    for (int c1 = 0; c1 < chunkLayer.GetLength(1); c1++)
                    {
                        //grab the list of explict chunks from the dictionary indexed by the chunk saved at the location in the chunk layer
                        List<char[,]> listOfExplicitChunks = explicitChunkDict[ chunkLayer[r1,c1] ];
                        //choose one explicit chunk at "random"
                        char[,] explicitChunk = listOfExplicitChunks[_random.Next(0, listOfExplicitChunks.Count)];
                        //load the explicit chunk into the final explicit layer
                        for (int r2 = 0; r2 < explicitChunk.GetLength(0); r2++)
                            for (int c2 = 0; c2 < explicitChunk.GetLength(1); c2++)
                                explicitLayer[r1 * explicitChunkRows + r2, c1 * explicitChunkCols + c2] = explicitChunk[r2, c2];
                    }
                //Store the explicit layer into the list of explicit layers
                explicitLayers.Add(explicitLayer);
            }
            //store the number and rows and cols in the final level by grabing the rows and cols of the explicit layers
            int finalLevelRows = explicitLayers[0].GetLength(0) * explicitLayers.Count;
            int finalLevelCols = explicitLayers[0].GetLength(1);

            //Instantiate the final usable explicit level
            char[,] finalLevel = new char[finalLevelRows, finalLevelCols];
            //combine all the layers into the final level
            for (int i = 0; i < explicitLayers.Count; i++)
                for (int r = 0; r < finalLevelRows / explicitLayers.Count; r++)
                    for (int c = 0; c < finalLevelCols; c++)
                        finalLevel[r*i, c] = explicitLayers[i][r, c];
            //return the level 
            return finalLevel;

        }
        #endregion
            #region update old maps and chunks
        /// <summary>
        /// Opens up old maps, replaces source characters with target characters from a dictionary of the form: 
        /// dict[sourceChar]=targetChar
        /// </summary>
        /// <param name="replacementDict"></param>
        public void UpdateOldMaps(Dictionary<char, char> replacementDict)
            {
                //for each map
                //open map
                //for each sourceChar in replacementDict
                //text.Replace('sourceChar','targetChar');
                //save map over old map
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            #endregion
            #region IO
        public void SaveCharMatrixToTextFile(char[,] matrix, string filename)
        {
            try
            {
                using(StreamWriter writer = new StreamWriter(filename))
                {
                    for (int r = 0; r < matrix.GetLength(0); r++)
                    {
                        String line = "";
                        for (int c = 0; c < matrix.GetLength(1); c++)
                        {
                            writer.Write(matrix[r, c]);
                            Console.Write(matrix[r, c]);
                        }
                        writer.WriteLine();
                        Console.WriteLine();
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error Saving to " + filename);
            }
        }
        #endregion  
        #endregion
    }

}
