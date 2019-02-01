#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("oR3raY+XjlfPAO1IB5Y3EwOcM0X1kIE5p6v1X5/0lvS7jZrjsdd65sXoJ7x6qccakytgOs/G01iGEk0HaLRJ0AmQl5xBWFCF7ciTTPnZeMyeLK+MnqOop4Qo5ihZo6+vr6uurY9bPrRblNoMOKH0RiP83bMfD+MILK+hrp4sr6SsLK+vri7p5YpAfsFOtvvNbozazOAM7QZfOQzOQYNcpwRv8uJOtn3xhg2zK7ic18Cv0/G7N8tXJkTCcVV448iv+x56QcdJvo4Ia9ORKMKOYNq+kdHyC7Y+txKTtI9AqIpXutDP3B5XxzJAKNS5Xf3MRez3bR4Xg07fNsZSK5xhkb/29w7WoazdAq0aqD7aDzpbFX3SLLibF8WrusAGJBRnm6ytr66v");
        private static int[] order = new int[] { 7,8,5,5,7,6,8,12,10,10,11,13,12,13,14 };
        private static int key = 174;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
