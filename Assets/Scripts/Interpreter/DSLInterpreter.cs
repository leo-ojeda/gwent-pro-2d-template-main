//using UnityEngine;
//using UnityEngine.UI;
//using DSL.Lexer;
//using DSL.Parser;
//using System;
//using TMPro;
//
//public class DSLInterpreter : MonoBehaviour
//{
//    public TMP_InputField inputField;
//    public Text outputText;
//
//    public void InterpretDSL()
//    {
//        string userInput = inputField.text;
//
//        try
//        {
//            LexerStream lexerStream = new LexerStream(userInput);
//            Parser parser = new Parser(lexerStream);
//           var cardNode = parser.ParseCard();
//
//            outputText.text = "Card parsed successfully: " + cardNode.ToString();
//        }
//        catch (Exception ex)
//        {
//            outputText.text = "Error parsing input: " + ex.Message;
//        }
//    }
//}
