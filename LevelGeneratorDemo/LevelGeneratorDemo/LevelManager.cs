using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            #region level generation
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
            //solution path's current position in the evel
            int[] pos = new int[2];
            pos[0] = _random.Next(0, LAYER_HEIGHT); //row position
            pos[1] = 0; //col position
            //move until the position has reached the end of the level
            while (pos[1] < LEVEL_WIDTH)
            {
                //generate the chunk type for the current position
                int chunkType = this.RandomChunkType();
                //int chunkType = (int)(_random.NextDouble() * 4);
                //Console.WriteLine("{2}", pos[0], pos[1], chunkType);
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
                        layer[r, c] = RandomChunkType(0,0,0,1);
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
        #endregion
    }

}
