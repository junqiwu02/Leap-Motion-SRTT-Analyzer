﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TestMaker : MonoBehaviour {

    public MenuController menuController;

    // NewTestPanel fields
    public InputField nameField;
    public InputField blockField;
    public Toggle delayToggle;

    // NewBlockPanel fields
    public InputField sequenceField;
    public InputField repetitionField;

    public Text blockTitle;
    public Text nextButton;

    public GameObject mainPanel;
    public GameObject newBlockPanel;
    public GameObject newTestPanel;

    public GameObject blankFieldError;

    private string testName;
    private int numOfBlocks;
    private bool hasDelay;
    private string[] sequences;
    private int[] repetitions;

    private int currentBlock = 0;

    private StreamWriter writer;

    private string path;
    private string editorPath = "/Resources/Tests/";
    private string buildPath = "/Tests/";

    private void Start()
    {
        // set path
        if (Application.isEditor)
        {
            path = Application.dataPath + editorPath;
        }
        else
        {
            path = Application.dataPath + buildPath;
        }
    }

    // set the texts every frame
    private void Update()
    {
        blockTitle.text = "Block #" + (currentBlock + 1);
        if(currentBlock == numOfBlocks - 1)
        {
            nextButton.text = "Finish";
        }
        else
        {
            nextButton.text = "Next";
        }
    }

    // reset all the variables
    public void ClearTest()
    {
        testName = null;
        numOfBlocks = 0;
        hasDelay = false;
        sequences = null;
        repetitions = null;
        currentBlock = 0;
    }

    // create a test with empty blocks based on user input
    public void CreateTest()
    {
        if(nameField.text != "" && blockField.text != "")
        {
            blankFieldError.SetActive(false);

            testName = nameField.text;
            Int32.TryParse(blockField.text, out numOfBlocks);
            hasDelay = delayToggle.isOn;
            sequences = new string[numOfBlocks];
            repetitions = new int[numOfBlocks];

            currentBlock = 0;

            // clear input fields
            nameField.text = "";
            repetitionField.text = "";
            delayToggle.isOn = true;

            newTestPanel.SetActive(false);
            newBlockPanel.SetActive(true);
        }
        else
        {
            blankFieldError.SetActive(true);
        }
    }

    public void SetBlock()
    {
        if(sequenceField.text != "" && repetitionField.text != "")
        {
            blankFieldError.SetActive(false);

            // set current block
            sequences[currentBlock] = sequenceField.text;
            Int32.TryParse(repetitionField.text, out repetitions[currentBlock]);

            // clear input fields
            sequenceField.text = "";
            repetitionField.text = "";

            // increment current block or write file
            if (currentBlock == numOfBlocks - 1)
            {
                WriteFile();
            }
            else
            {
                currentBlock++;
            }
        }
        else
        {
            blankFieldError.SetActive(true);
        }
    }

    public void WriteFile()
    {
        // set path
        path += testName + ".csv";

        // clear file
        File.WriteAllText(path, "");
        writer = new StreamWriter(path, true);

        // write
        writer.WriteLine(numOfBlocks + "," + hasDelay);
        for(int i = 0; i < sequences.Length; i++)
        {
            writer.WriteLine(sequences[i] + "," + repetitions[i]);
        }

        // clear test, update dropdown, and go to main panel
        writer.Close();
        ClearTest();
        menuController.SetTests();
        newBlockPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
