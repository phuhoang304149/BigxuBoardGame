#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("08XR8/ag8f7m6LSFhZmQ1baQh4HjxeHz9qDx9ub4tIWFmZDVp5qagUTFrRmv8cd5nUZ66CuQhgqSq5BJ1ZSbkdWWkIeBnJOclpSBnJqb1YXx8+b3oKbE5sXk8/ag8f/m/7SFhUsBhm4bJ5H6Poy6wS1XywyNCp49irRdbQwkP5Np0Z7kJVZOEe7fNur48/zfc71zAvj09PDw9fZ39PT1qYKC25SFhZmQ25aamNqUhYWZkJaUw2y52I1CGHluKQaCbgeDJ4LFujR14d4lnLJhg/wLAZ5427VTArK4ioeUloGclpDVhoGUgZCYkJuBhtvFnJOclpSBnJqb1bSAgZ2ah5yBjMTzxfrz9qDo5vT0CvHwxfb09ArF6MV38U7Fd/ZWVfb39Pf39PfF+PP80RceJEKFKvqwFNI/BJiNGBJA4uKPxXf0g8X78/ag6Pr09Arx8fb39CzDijRyoCxSbEzHtw4tIIRri1SngZyTnJaUgZDVl4zVlJuM1YWUh4HGw6/Fl8T+xfzz9qDx8+b3oKbE5v3e8/Tw8PL39OPrnYGBhYbP2tqCm5HVlpqbkZyBnJqbhtWak9WAhpD9q8V39OTz9qDo1fF39P3Fd/TxxcjTktV/xp8C+Hc6Kx5W2gymn66R8PX2d/T69cV39P/3d/T09RFkXPzqcHZw7mzIssIHXG61e9khRGXnLdW2tMV39NfF+PP833O9cwL49PT0+mjIBt683e89CztATPssq+kjPsg1lsaCAs/y2aMeL/rU+y9Phuy6QNu1UwKyuIr9q8Xq8/ag6Nbx7cXj6mQu67KlHvAYq4xx2B7DV6K5oBmsUvD8ieK1o+TrgSZCftbOslYgmpJ6/UHVAj5Z2dWahUPK9MV5QrY6fux8Kwy+mQDyXtfF9x3tyw2l/CZ39PXz/N9zvXMClpHw9MV0B8Xf84zVlIaGgJiQhtWUlpaQhYGUm5aQQM9YAfr79Wf+RNTj24Egyfgul+OnkJmclJuWkNWam9WBnZyG1ZaQh7CL6rmepWO0fDGBl/7ldrRyxn901ZqT1YGdkNWBnZCb1ZSFhZmclpQ87IcAqPsgiqpuB9D2T6B6uKj4BGBrj/lRsn6uIePCxj4x+rg74ZwkQu5IZrfR598y+uhDuGmrlj2+deLz9qDo+/Hj8eHeJZyyYYP8CwGeeHqGdJUz7q782mdHDbG9BZXNa+AAl5mQ1YaBlJuRlIeR1YGQh5iG1ZTyGYjMdn6m1SbNMURKb7r/ngreCZmQ1bybltvE08XR8/ag8f7m6LSFXSmL18A/0CAs+iOeIVfR1uQCVFnAx8TBxcbDr+L4xsDFx8XMx8TBxV5WhGeypqA0Wtq0Rg0OFoU4E1a5vC2DasbhkFSCYTzY9/b09fRWd/SRwNbgvuCs6EZhAgNpazqlTzStpYWZkNW2kIeBnJOclpSBnJqb1bSA33O9cwL49PTw8PXFl8T+xfzz9qCFmZDVp5qagdW2tMXr4vjFw8XBx9rFdDbz/d7z9PDw8vf3xXRD73RG2dWWkIeBnJOclpSBkNWFmpmcloyBnZqHnIGMxOPF4fP2oPH25vi0hcXk8/ag8f/m/7SFhZmQ1bybltvEpV9/IC8RCSX88sJFgIDU");
        private static int[] order = new int[] { 10,12,26,45,26,50,17,47,36,48,32,46,46,29,32,53,28,58,39,36,35,43,33,56,32,49,29,33,57,31,53,52,43,57,34,40,56,38,49,41,56,41,59,53,55,59,51,59,54,59,53,57,56,56,57,58,59,58,59,59,60 };
        private static int key = 245;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
