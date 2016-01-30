using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIRD.PoissonDistribution
{
    public static class getPoissonDistribution
    {
        public static double UpperLimit(int observed)
        {
            switch (observed)
            {
                case 0: { return 2.9957; }
                case 1: { return 5.5716; }
                case 2: { return 7.2247; }
                case 3: { return 8.7673; }
                case 4: { return 10.2416; }
                case 5: { return 11.6683; }
                case 6: { return 13.0595; }
                case 7: { return 14.4227; }
                case 8: { return 15.7632; }
                case 9: { return 17.0848; }
                case 10: { return 18.3904; }
                case 11: { return 19.682; }
                case 12: { return 20.9616; }
                case 13: { return 22.2304; }
                case 14: { return 23.4896; }
                case 15: { return 24.7402; }
                case 16: { return 25.983; }
                case 17: { return 27.2186; }
                case 18: { return 28.4478; }
                case 19: { return 29.6709; }
                case 20: { return 30.8884; }
                case 21: { return 32.1007; }
                case 22: { return 33.3083; }
                case 23: { return 34.5113; }
                case 24: { return 35.7101; }
                case 25: { return 36.9049; }
                case 26: { return 38.096; }
                case 27: { return 39.2836; }
                case 28: { return 40.4678; }
                case 29: { return 41.6488; }
                case 30: { return 42.8269; }
                case 31: { return 44.002; }
                case 32: { return 45.1745; }
                case 33: { return 46.3443; }
                case 34: { return 47.5116; }
                case 35: { return 48.6765; }
                case 36: { return 49.8392; }
                case 37: { return 50.9996; }
                case 38: { return 52.158; }
                case 39: { return 53.3143; }
                case 40: { return 54.4686; }
                case 41: { return 55.6211; }
                case 42: { return 56.7718; }
                case 43: { return 57.9207; }
                case 44: { return 59.0679; }
                case 45: { return 60.2135; }
                case 46: { return 61.358; }
                case 47: { return 62.5; }
                case 48: { return 63.641; }
                case 49: { return 64.781; }
                case 50: { return 65.919; }
                case 51: { return 67.056; }
                case 52: { return 68.191; }
                case 53: { return 69.325; }
                case 54: { return 70.458; }
                case 55: { return 71.59; }
                case 56: { return 72.721; }
                case 57: { return 73.85; }
                case 58: { return 74.978; }
                case 59: { return 76.106; }
                case 60: { return 77.232; }
                case 61: { return 78.357; }
                case 62: { return 79.481; }
                case 63: { return 80.604; }
                case 64: { return 81.727; }
                case 65: { return 82.848; }
                case 66: { return 83.968; }
                case 67: { return 85.088; }
                case 68: { return 86.206; }
                case 69: { return 87.324; }
                case 70: { return 88.441; }
                case 71: { return 89.557; }
                case 72: { return 90.672; }
                case 73: { return 91.787; }
                case 74: { return 92.9; }
                case 75: { return 94.013; }
                case 76: { return 95.125; }
                case 77: { return 96.237; }
                case 78: { return 97.348; }
                case 79: { return 98.458; }
                case 80: { return 99.567; }
                case 81: { return 100.676; }
                case 82: { return 101.784; }
                case 83: { return 102.891; }
                case 84: { return 103.998; }
                case 85: { return 105.104; }
                case 86: { return 106.209; }
                case 87: { return 107.314; }
                case 88: { return 108.418; }
                case 89: { return 109.522; }
                case 90: { return 110.625; }
                case 91: { return 111.728; }
                case 92: { return 112.83; }
                case 93: { return 113.931; }
                case 94: { return 115.032; }
                case 95: { return 116.133; }
                case 96: { return 117.232; }
                case 97: { return 118.332; }
                case 98: { return 119.431; }
                case 99: { return 120.529; }

            }
            return -1;
        }
        public static double LowerLimit(int observed)
        {
            switch (observed)
            {
                case 0: { return 0; }
                case 1: { return 0.0253; }
                case 2: { return 0.2422; }
                case 3: { return 0.6187; }
                case 4: { return 1.0899; }
                case 5: { return 1.6235; }
                case 6: { return 2.2019; }
                case 7: { return 2.8144; }
                case 8: { return 3.4538; }
                case 9: { return 4.1154; }
                case 10: { return 4.7954; }
                case 11: { return 5.4912; }
                case 12: { return 6.2006; }
                case 13: { return 6.922; }
                case 14: { return 7.6539; }
                case 15: { return 8.3954; }
                case 16: { return 9.1454; }
                case 17: { return 9.9031; }
                case 18: { return 10.6679; }
                case 19: { return 11.4392; }
                case 20: { return 12.2165; }
                case 21: { return 12.9993; }
                case 22: { return 13.7873; }
                case 23: { return 14.58; }
                case 24: { return 15.3773; }
                case 25: { return 16.1787; }
                case 26: { return 16.9841; }
                case 27: { return 17.7932; }
                case 28: { return 18.6058; }
                case 29: { return 19.4218; }
                case 30: { return 20.2409; }
                case 31: { return 21.063; }
                case 32: { return 21.888; }
                case 33: { return 22.7157; }
                case 34: { return 23.546; }
                case 35: { return 24.3788; }
                case 36: { return 25.214; }
                case 37: { return 26.0514; }
                case 38: { return 26.8911; }
                case 39: { return 27.7328; }
                case 40: { return 28.5766; }
                case 41: { return 29.4223; }
                case 42: { return 30.2699; }
                case 43: { return 31.1193; }
                case 44: { return 31.9705; }
                case 45: { return 32.8233; }
                case 46: { return 33.6778; }
                case 47: { return 34.5338; }
                case 48: { return 35.3914; }
                case 49: { return 36.2505; }
                case 50: { return 37.111; }
                case 51: { return 37.9728; }
                case 52: { return 38.8361; }
                case 53: { return 39.7006; }
                case 54: { return 40.5665; }
                case 55: { return 41.4335; }
                case 56: { return 42.3018; }
                case 57: { return 43.1712; }
                case 58: { return 44.0418; }
                case 59: { return 44.9135; }
                case 60: { return 45.7863; }
                case 61: { return 46.6602; }
                case 62: { return 47.535; }
                case 63: { return 48.4109; }
                case 64: { return 49.2878; }
                case 65: { return 50.1656; }
                case 66: { return 51.0444; }
                case 67: { return 51.9241; }
                case 68: { return 52.8047; }
                case 69: { return 53.6861; }
                case 70: { return 54.5684; }
                case 71: { return 55.4516; }
                case 72: { return 56.3356; }
                case 73: { return 57.2203; }
                case 74: { return 58.1059; }
                case 75: { return 58.9923; }
                case 76: { return 59.8794; }
                case 77: { return 60.7672; }
                case 78: { return 61.6558; }
                case 79: { return 62.545; }
                case 80: { return 63.435; }
                case 81: { return 64.3257; }
                case 82: { return 65.217; }
                case 83: { return 66.109; }
                case 84: { return 67.0017; }
                case 85: { return 67.895; }
                case 86: { return 68.7889; }
                case 87: { return 69.6834; }
                case 88: { return 70.5786; }
                case 89: { return 71.4743; }
                case 90: { return 72.3706; }
                case 91: { return 73.2675; }
                case 92: { return 74.165; }
                case 93: { return 75.063; }
                case 94: { return 75.9616; }
                case 95: { return 76.8607; }
                case 96: { return 77.7603; }
                case 97: { return 78.6605; }
                case 98: { return 79.5611; }
                case 99: { return 80.4623; }

            }
            return -1;
        }
    }
}
