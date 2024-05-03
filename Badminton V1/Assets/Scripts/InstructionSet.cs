using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionSet
{
    public List<string> instructions = new List<string>();
    public int indexOfInstructionOn = -1;

    public InstructionSet(string instructionSetName)
    {
        if (instructionSetName == "starterInstructions")
        {
            AddInstruction("Move the thumbstick right to progress the instructions. Move it left to go back.");
            AddInstruction("Welcome to VR badminton!");
            AddInstruction("In all following scenarios, a virtual avatar will hit smashes towards you.");
            AddInstruction("There are 3 scenarios in total: First Test, Training, Final Test.");
            AddInstruction("The exact purpose of each scenario will be explained in further instructions, directly before each scenario takes place.");
            AddInstruction("In each scenario it is your goal to retrieve smashes to the best of your ability, as you would in a normal game.");
            AddInstruction("Smashes will all be within reach, and will be hit to a random side each time.");
            AddInstruction("You will only be given 5 seconds between smashes, so make sure you are prepared.");
            AddInstruction("You will now complete the first test scenario.");
            AddInstruction("Here you will need to return 5 smashes to the best of your ability.");
            AddInstruction("When you are ready, move your thumbstick to the right to start.");
        }
        else if (instructionSetName == "TTInstructions")
        {
            AddInstruction("Now you will undergo training.");
            AddInstruction("Please read the following training tips carefully, and try to implement them.");
            AddInstruction("Keep a low stance. Prepare your racket as early as possible. Keep your body relaxed. ");
            AddInstruction("You will now be given 30 smashes to return as training.");
            AddInstruction("When you are ready, move your thumbstick to the right to start.");
        }
        else if (instructionSetName == "QETInstructions")
        {
            AddInstruction("Now you will undergo training.");
            AddInstruction("This training scenario is identical to the previous test scenario except for a key difference.");
            AddInstruction("Once the avatar hits the shuttlecock, a randomly coloured sphere will briefly appear at the hit point.");
            AddInstruction("You must say the colour of the sphere out loud before you hit the shuttlecock.");
            AddInstruction("To the best of your ability, you should try and look directly at the sphere, rather than using your peripheral vision.");
            AddInstruction("You will now be given 30 smashes to return as training.");
            AddInstruction("When you are ready, move your thumbstick to the right to start.");
        }
        else if (instructionSetName == "finalTestInstructions")
        {
            AddInstruction("You will now complete the final test scenario. It is identical to the first test scenario.");
            AddInstruction("You will need to return 5 smashes to the best of your ability.");
            AddInstruction("When you are ready, move your thumbstick to the right to start.");
        }
        else if (instructionSetName == "debriefTestInstructions")
        {
            AddInstruction("That is the end of the VR section of the study. You may now remove the VR headset.");
        }

        indexOfInstructionOn = 0;
    }

    public void AddInstruction(string instruction)
    {
        instructions.Add(instruction);

        if (instructions.Count == 1)
        {
            indexOfInstructionOn = 0;
        }
    }

    public string GetCurrentInstruction()
    {
        if (indexOfInstructionOn >= 0 && indexOfInstructionOn < instructions.Count)
        {
            return instructions[indexOfInstructionOn];
        }
        else
        {
            return "";
        }
    }

    public bool MoveToNextInstruction()
    {
        if (indexOfInstructionOn < instructions.Count - 1)
        {
            indexOfInstructionOn++;
            return true;
        }

        return false;
    }

    public bool MoveToPreviousInstruction()
    {
        if (indexOfInstructionOn > 0)
        {
            indexOfInstructionOn--;
            return true;
        }

        return false;
    }
}
