using System;


namespace Propolis {
    public class HexData: PropolisDataType
    {
        public HexData(int id ) : base()
        {
            Status = 0;
            ID = id;
            Error = false;
        }
        public HexData (string[] modelParams) : base()
        {
            Error = true;
            int parsingStatusValue;
            if (modelParams.Length == 2)
            {
                try
                {
                    ID = Convert.ToInt32(modelParams[0]);
                    if (!int.TryParse(modelParams[1],out parsingStatusValue))
                    {
                        parsingStatusValue = (int)Enum.Parse(typeof(PropolisStatus), modelParams[1]);
                        Status = parsingStatusValue;
                        Error = false;
                    }                
                  
                }
                catch
                {
                    return;
                }

                Error = false;

            }
        }
        public int Status { get; set; }
    }
}

