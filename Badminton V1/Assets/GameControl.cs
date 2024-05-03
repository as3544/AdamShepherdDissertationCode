using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class GameControl : MonoBehaviour
{

    InstructionSet starterInstructions;
    InstructionSet trainingInstructions;
    InstructionSet finalTestInstructions;
    InstructionSet debriefTestInstructions;

    List<QEFixation> qeFixations = new List<QEFixation>();

    List<InstructionSet> instructionSetOrder = new List<InstructionSet>();

    InstructionSet currentInstructionSet;

    public GameObject instructionTextObject;

    public EyeGazeController eyeGazeController;
    public ShuttlecockLauncher launcher;
    public bool participantLeftHanded = false;
    public bool QET = true;

    bool runningRepetitions = false;


    long timeSinceLastInput = 0;

    // instruction, how many iterations to complete, whether QET is enabled
    List<(InstructionSet, int, bool)> instructionToRepetitionsMap = new List<(InstructionSet, int, bool)>();


    private void Start()
    {

        launcher.playerLeftHanded = participantLeftHanded;

        starterInstructions = new InstructionSet("starterInstructions");
        if (QET)
        {
            trainingInstructions = new InstructionSet("QETInstructions");
        }
        else
        {
            trainingInstructions = new InstructionSet("TTInstructions");
        }
        finalTestInstructions = new InstructionSet("finalTestInstructions");
        debriefTestInstructions = new InstructionSet("debriefTestInstructions");

        currentInstructionSet = starterInstructions;
        SetTextToCurrentInstruction();

        instructionToRepetitionsMap.Add((starterInstructions, 5, false));
        instructionToRepetitionsMap.Add((trainingInstructions, 30, QET));
        instructionToRepetitionsMap.Add((finalTestInstructions, 5, false));
        instructionToRepetitionsMap.Add((debriefTestInstructions, 0, false));
        instructionSetOrder = new List<InstructionSet>() { starterInstructions, trainingInstructions, finalTestInstructions, debriefTestInstructions };
    }

    private void Update()
    {
        Vector2 input = participantLeftHanded ? OVRInput.Get(OVRInput.RawAxis2D.LThumbstick) : OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 250 > timeSinceLastInput)
        {
            if (input.x > 0.75f)
            {
                MoveToNextInstruction();
                timeSinceLastInput = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            else if (input.x < -0.75f)
            {
                MoveToPreviousInstruction();
                timeSinceLastInput = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }
    }

    private void MoveToNextInstruction()
    {
        if (runningRepetitions) return;

        bool success = currentInstructionSet.MoveToNextInstruction();
        
        if (success)
        {
            SetTextToCurrentInstruction();
        }
        else
        {
            (InstructionSet, int, bool) instructionAssociatedRepetitions = instructionToRepetitionsMap.Find(x => x.Item1.instructions.Equals(currentInstructionSet.instructions));

            GetComponent<Canvas>().enabled = false;
            runningRepetitions = true;
            launcher.RunRepetitions(instructionAssociatedRepetitions.Item2, instructionAssociatedRepetitions.Item3);
        }
    }

    private void MoveToPreviousInstruction()
    {
        if (runningRepetitions) return;

        bool success = currentInstructionSet.MoveToPreviousInstruction();

        if (success)
        {
            SetTextToCurrentInstruction();
        }
    }

    private void SetTextToCurrentInstruction()
    {
        instructionTextObject.GetComponent<TextMeshProUGUI>().text = currentInstructionSet.GetCurrentInstruction();
    }

    // later will pass in fixations as a parameter to be saved, matched up or calculated.
    public void RepetitionsFinished()
    {
        currentInstructionSet = instructionSetOrder[instructionSetOrder.IndexOf(currentInstructionSet) + 1];
        GetComponent<Canvas>().enabled = true;
        SetTextToCurrentInstruction();

        // ADD EYE TRACKING REPETITIONS TO LIST
        foreach (long timestamp in launcher.shuttleHitPointTimeStamps)
        {
            // get fixation closest to timestamp
            Fixation closestFixation = eyeGazeController.fixationTracker.GetFixationStartingClosestToBeforeTime(timestamp);

            qeFixations.Add(new QEFixation(closestFixation, timestamp));
        }

        ExportQEDataToCSV("C:\\Users\\adam-\\Desktop\\QEFixationsFolder\\QE" + DateTime.Now.ToLongTimeString().Replace(":", "") + ".csv");

        // clear out list now we've transferred over
        launcher.shuttleHitPointTimeStamps.Clear();
        runningRepetitions = false;
    }

    void ExportQEDataToCSV(string saveLocation)
    {
        StringBuilder csv = new StringBuilder();

        for (int i = 0; i < qeFixations.Count; i++)
        {
            QEFixation fixation = qeFixations[i];

            //in your loop
            string index = i.ToString();
            string fixationLocationX = fixation.fixation.gazeLocation.x.ToString();
            string fixationLocationY = fixation.fixation.gazeLocation.y.ToString();
            string fixationLocationZ = fixation.fixation.gazeLocation.z.ToString();
            string hitpoint = fixation.relativeHitPointTimeStamp.ToString();
            string onset = fixation.onset.ToString();
            string offset = fixation.offset.ToString();
            string duration = fixation.duration.ToString();

            string newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", index, fixationLocationX, fixationLocationY, fixationLocationZ, hitpoint, onset, offset, duration);
            csv.AppendLine(newLine);
        }
        File.WriteAllText(saveLocation, csv.ToString());
    }
}
