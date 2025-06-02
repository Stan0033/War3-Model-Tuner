using MdxLib.Primitives;
using System;
using System.Collections.Generic;

namespace Wa3Tuner.Helper_Classes
{
    public static class LetterShapes
    {
        public static Dictionary<char, List<CVector3>> Shapes = new Dictionary<char, List<CVector3>>()
        {
            // Uppercase Letters
            { 'A', new List<CVector3> { new CVector3(0, 0, 0), new CVector3(1, 2, 0), new CVector3(2, 0, 0), new CVector3(0.5f, 1, 0), new CVector3(1.5f, 1, 0) } },
            { 'B', new List<CVector3> { new CVector3(0, 0, 0), new CVector3(0, 2, 0), new CVector3(1, 2, 0), new CVector3(1.2f, 1.5f, 0), new CVector3(1, 1, 0), new CVector3(1.2f, 0.5f, 0), new CVector3(1, 0, 0) } },
            { 'C', new List<CVector3> { new CVector3(1, 2, 0), new CVector3(0, 1.5f, 0), new CVector3(0, 0.5f, 0), new CVector3(1, 0, 0) } },
            { 'D', new List<CVector3> { new CVector3(0, 0, 0), new CVector3(0, 2, 0), new CVector3(1, 1.5f, 0), new CVector3(1, 0.5f, 0), new CVector3(0, 0, 0) } },
            { 'E', new List<CVector3> { new CVector3(1, 2, 0), new CVector3(0, 2, 0), new CVector3(0, 0, 0), new CVector3(1, 0, 0), new CVector3(0, 1, 0), new CVector3(0.7f, 1, 0) } },
            { 'F', new List<CVector3> { new CVector3(0, 0, 0), new CVector3(0, 2, 0), new CVector3(1, 2, 0), new CVector3(0, 1, 0), new CVector3(0.7f, 1, 0) } },
            { 'G', new List<CVector3> { new CVector3(1, 2, 0), new CVector3(0, 1.5f, 0), new CVector3(0, 0.5f, 0), new CVector3(1, 0, 0), new CVector3(1, 1, 0), new CVector3(0.5f, 1, 0) } },
            { 'H', new List<CVector3> { new CVector3(0, 0, 0), new CVector3(0, 2, 0), new CVector3(0, 1, 0), new CVector3(1, 1, 0), new CVector3(1, 0, 0), new CVector3(1, 2, 0) } },
            { 'I', new List<CVector3> { new CVector3(0, 2, 0), new CVector3(1, 2, 0), new CVector3(0.5f, 2, 0), new CVector3(0.5f, 0, 0), new CVector3(0, 0, 0), new CVector3(1, 0, 0) } },
            { 'J', new List<CVector3> { new CVector3(1, 2, 0), new CVector3(1, 0.5f, 0), new CVector3(0.5f, 0, 0), new CVector3(0, 0.5f, 0) } },
            // (continue adding all uppercase...)

            // Lowercase Letters
            { 'a', new List<CVector3> { new CVector3(1, 1, 0), new CVector3(0.5f, 1.5f, 0), new CVector3(0, 1, 0), new CVector3(0.5f, 0.5f, 0), new CVector3(1, 1, 0) } },
            { 'b', new List<CVector3> { new CVector3(0, 0, 0), new CVector3(0, 2, 0), new CVector3(1, 1.5f, 0), new CVector3(1, 0.5f, 0), new CVector3(0, 0, 0) } },
            { 'c', new List<CVector3> { new CVector3(1, 1.5f, 0), new CVector3(0.5f, 2, 0), new CVector3(0, 1.5f, 0), new CVector3(0, 0.5f, 0), new CVector3(0.5f, 0, 0), new CVector3(1, 0.5f, 0) } },
            { 'd', new List<CVector3> { new CVector3(1, 0, 0), new CVector3(1, 2, 0), new CVector3(0, 1.5f, 0), new CVector3(0, 0.5f, 0), new CVector3(1, 0, 0) } },
            { 'e', new List<CVector3> { new CVector3(0, 1, 0), new CVector3(1, 1, 0), new CVector3(0.5f, 2, 0), new CVector3(0, 1.5f, 0), new CVector3(0, 0.5f, 0), new CVector3(0.5f, 0, 0), new CVector3(1, 0.5f, 0) } },
            { 'f', new List<CVector3> { new CVector3(0.5f, 0, 0), new CVector3(0.5f, 2, 0), new CVector3(0, 1.5f, 0), new CVector3(1, 1.5f, 0) } },
            { 'g', new List<CVector3> { new CVector3(1, 1, 0), new CVector3(0.5f, 1.5f, 0), new CVector3(0, 1, 0), new CVector3(0.5f, 0.5f, 0), new CVector3(1, 1, 0), new CVector3(1, 0.5f, 0), new CVector3(0.5f, 0, 0) } },
            { 'h', new List<CVector3> { new CVector3(0, 0, 0), new CVector3(0, 2, 0), new CVector3(0, 1, 0), new CVector3(1, 1, 0), new CVector3(1, 0, 0) } },
            { 'i', new List<CVector3> { new CVector3(0.5f, 0, 0), new CVector3(0.5f, 1, 0), new CVector3(0.5f, 1.5f, 0) } },
            { 'j', new List<CVector3> { new CVector3(1, 1.5f, 0), new CVector3(1, 0.5f, 0), new CVector3(0.5f, 0, 0), new CVector3(0, 0.5f, 0) } },
            // (continue adding all lowercase...)

        

{ 'K', new List<CVector3> {
    new CVector3(0, 0, 0),
    new CVector3(0, 2, 0),
    new CVector3(1, 1, 0),
    new CVector3(0, 1, 0),
    new CVector3(1, 2, 0),
}},

{ 'L', new List<CVector3> {
    new CVector3(0, 2, 0),
    new CVector3(0, 0, 0),
    new CVector3(1, 0, 0),
}},

{ 'M', new List<CVector3> {
    new CVector3(0, 0, 0),
    new CVector3(0, 2, 0),
    new CVector3(1, 1, 0),
    new CVector3(2, 2, 0),
    new CVector3(2, 0, 0),
}},

{ 'N', new List<CVector3> {
    new CVector3(0, 0, 0),
    new CVector3(0, 2, 0),
    new CVector3(2, 0, 0),
    new CVector3(2, 2, 0),
}},

{ 'O', new List<CVector3> {
    new CVector3(0, 0.5f, 0),
    new CVector3(0.5f, 0, 0),
    new CVector3(1.5f, 0, 0),
    new CVector3(2, 0.5f, 0),
    new CVector3(2, 1.5f, 0),
    new CVector3(1.5f, 2, 0),
    new CVector3(0.5f, 2, 0),
    new CVector3(0, 1.5f, 0),
}},

{ 'P', new List<CVector3> {
    new CVector3(0, 0, 0),
    new CVector3(0, 2, 0),
    new CVector3(1, 2, 0),
    new CVector3(1.2f, 1.5f, 0),
    new CVector3(1, 1, 0),
    new CVector3(0, 1, 0),
}},

{ 'Q', new List<CVector3> {
    new CVector3(0, 0.5f, 0),
    new CVector3(0.5f, 0, 0),
    new CVector3(1.5f, 0, 0),
    new CVector3(2, 0.5f, 0),
    new CVector3(2, 1.5f, 0),
    new CVector3(1.5f, 2, 0),
    new CVector3(0.5f, 2, 0),
    new CVector3(0, 1.5f, 0),
    new CVector3(1.2f, 0.8f, 0),
    new CVector3(2, 0, 0),
}},

{ 'R', new List<CVector3> {
    new CVector3(0, 0, 0),
    new CVector3(0, 2, 0),
    new CVector3(1, 2, 0),
    new CVector3(1.2f, 1.5f, 0),
    new CVector3(1, 1, 0),
    new CVector3(0, 1, 0),
    new CVector3(1.2f, 0, 0),
}},

{ 'S', new List<CVector3> {
    new CVector3(1.5f, 2, 0),
    new CVector3(0.5f, 2, 0),
    new CVector3(0, 1.5f, 0),
    new CVector3(0.5f, 1, 0),
    new CVector3(1.5f, 1, 0),
    new CVector3(2, 0.5f, 0),
    new CVector3(1.5f, 0, 0),
    new CVector3(0.5f, 0, 0),
}},

{ 'T', new List<CVector3> {
    new CVector3(0, 2, 0),
    new CVector3(2, 2, 0),
    new CVector3(1, 2, 0),
    new CVector3(1, 0, 0),
}},

{ 'U', new List<CVector3> {
    new CVector3(0, 2, 0),
    new CVector3(0, 0.5f, 0),
    new CVector3(0.5f, 0, 0),
    new CVector3(1.5f, 0, 0),
    new CVector3(2, 0.5f, 0),
    new CVector3(2, 2, 0),
}},

{ 'V', new List<CVector3> {
    new CVector3(0, 2, 0),
    new CVector3(1, 0, 0),
    new CVector3(2, 2, 0),
}},

{ 'W', new List<CVector3> {
    new CVector3(0, 2, 0),
    new CVector3(0.5f, 0, 0),
    new CVector3(1, 1, 0),
    new CVector3(1.5f, 0, 0),
    new CVector3(2, 2, 0),
}},

{ 'X', new List<CVector3> {
    new CVector3(0, 2, 0),
    new CVector3(2, 0, 0),
    new CVector3(1, 1, 0),
    new CVector3(2, 2, 0),
    new CVector3(0, 0, 0),
}},

{ 'Y', new List<CVector3> {
    new CVector3(0, 2, 0),
    new CVector3(1, 1, 0),
    new CVector3(2, 2, 0),
    new CVector3(1, 1, 0),
    new CVector3(1, 0, 0),
}},

{ 'Z', new List<CVector3> {
    new CVector3(0, 2, 0),
    new CVector3(2, 2, 0),
    new CVector3(0, 0, 0),
    new CVector3(2, 0, 0),
}},


            // Digits
            { '0', new List<CVector3> { new CVector3(0, 0, 0), new CVector3(1, 0, 0), new CVector3(1, 2, 0), new CVector3(0, 2, 0), new CVector3(0, 0, 0) } },
            { '1', new List<CVector3> { new CVector3(0.5f, 0, 0), new CVector3(0.5f, 2, 0) } },
            { '2', new List<CVector3> { new CVector3(0, 1.5f, 0), new CVector3(0.5f, 2, 0), new CVector3(1, 1.5f, 0), new CVector3(0, 0, 0), new CVector3(1, 0, 0) } },
            { '3', new List<CVector3> { new CVector3(0, 2, 0), new CVector3(1, 2, 0), new CVector3(0.5f, 1, 0), new CVector3(1, 0, 0), new CVector3(0, 0, 0) } },
            { '4', new List<CVector3> { new CVector3(1, 0, 0), new CVector3(1, 2, 0), new CVector3(0, 1, 0), new CVector3(1, 1, 0) } },
            { '5', new List<CVector3> { new CVector3(1, 2, 0), new CVector3(0, 2, 0), new CVector3(0, 1, 0), new CVector3(1, 1, 0), new CVector3(1, 0, 0), new CVector3(0, 0, 0) } },
            { '6', new List<CVector3> { new CVector3(1, 2, 0), new CVector3(0, 1, 0), new CVector3(0, 0, 0), new CVector3(1, 0, 0), new CVector3(1, 1, 0), new CVector3(0, 1, 0) } },
            { '7', new List<CVector3> { new CVector3(0, 2, 0), new CVector3(1, 2, 0), new CVector3(0.5f, 0, 0) } },
            { '8', new List<CVector3> { new CVector3(0.5f, 1, 0), new CVector3(1, 2, 0), new CVector3(0, 2, 0), new CVector3(0.5f, 1, 0), new CVector3(1, 0, 0), new CVector3(0, 0, 0), new CVector3(0.5f, 1, 0) } },
            { '9', new List<CVector3> { new CVector3(1, 1, 0), new CVector3(0, 1, 0), new CVector3(0, 2, 0), new CVector3(1, 2, 0), new CVector3(1, 0, 0) } },
        };

        
    }
}
