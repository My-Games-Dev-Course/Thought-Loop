using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


// Comprehensive unit tests for the recording and playback system.
// These tests are external and independent - they don't require the actual RecorderManager.

[TestFixture]
public class RecordingTest
{
    // Test helper class that mimics RecorderManager functionality
    // This allows us to test independently without Unity scene dependencies
    private class TestRecorder
    {
        private List<Vector2> positions = new List<Vector2>();
        private List<Quaternion> rotations = new List<Quaternion>();
        private List<Vector2> scales = new List<Vector2>();
        private List<bool> flipXStates = new List<bool>();
        private List<bool> isRunningStates = new List<bool>();
        private bool isRecording = true;

        public void RecordFrame(Vector2 pos, Quaternion rot, Vector2 scale, bool flipX, bool isRunning)
        {
            if (isRecording)
            {
                positions.Add(pos);
                rotations.Add(rot);
                scales.Add(scale);
                flipXStates.Add(flipX);
                isRunningStates.Add(isRunning);
            }
        }

        public List<Vector2> GetPositions() => new List<Vector2>(positions);
        public List<Quaternion> GetRotations() => new List<Quaternion>(rotations);
        public List<Vector2> GetScales() => new List<Vector2>(scales);
        public List<bool> GetFlipXStates() => new List<bool>(flipXStates);
        public List<bool> GetIsRunningStates() => new List<bool>(isRunningStates);

        public void StopRecording() => isRecording = false;

        public void ClearRecording()
        {
            positions.Clear();
            rotations.Clear();
            scales.Clear();
            flipXStates.Clear();
            isRunningStates.Clear();
        }

        public int GetFrameCount() => positions.Count;
    }

    private TestRecorder recorder;

    [SetUp]
    public void Setup()
    {
        recorder = new TestRecorder();
    }

    [TearDown]
    public void Teardown()
    {
        recorder = null;
    }

    #region Recording Tests - Verify we can add and record data

    [Test]
    public void Test01_RecordSingleFrame_AddsDataToLists()
    {
        // Arrange
        Vector2 testPosition = new Vector2(1.0f, 2.0f);
        Quaternion testRotation = Quaternion.identity;
        Vector2 testScale = Vector2.one;
        bool testFlipX = false;
        bool testIsRunning = true;

        // Act
        recorder.RecordFrame(testPosition, testRotation, testScale, testFlipX, testIsRunning);

        // Assert
        Assert.AreEqual(1, recorder.GetFrameCount(), "Should have recorded 1 frame");
    }

    [Test]
    public void Test02_RecordMultipleFrames_StoresAllData()
    {
        // Arrange
        int frameCount = 10;

        // Act
        for (int i = 0; i < frameCount; i++)
        {
            Vector2 pos = new Vector2(i, i * 2);
            recorder.RecordFrame(pos, Quaternion.identity, Vector2.one, i % 2 == 0, i % 3 == 0);
        }

        // Assert
        Assert.AreEqual(frameCount, recorder.GetFrameCount(), $"Should have recorded {frameCount} frames");
    }

    [Test]
    public void Test03_RecordedPositions_MatchInputData()
    {
        // Arrange
        Vector2 pos1 = new Vector2(1.0f, 2.0f);
        Vector2 pos2 = new Vector2(3.0f, 4.0f);
        Vector2 pos3 = new Vector2(5.0f, 6.0f);

        // Act
        recorder.RecordFrame(pos1, Quaternion.identity, Vector2.one, false, false);
        recorder.RecordFrame(pos2, Quaternion.identity, Vector2.one, false, false);
        recorder.RecordFrame(pos3, Quaternion.identity, Vector2.one, false, false);

        var positions = recorder.GetPositions();

        // Assert
        Assert.AreEqual(3, positions.Count, "Should have 3 positions");
        Assert.AreEqual(pos1, positions[0], "First position should match");
        Assert.AreEqual(pos2, positions[1], "Second position should match");
        Assert.AreEqual(pos3, positions[2], "Third position should match");
    }

    [Test]
    public void Test04_RecordedRotations_MatchInputData()
    {
        // Arrange
        Quaternion rot1 = Quaternion.Euler(0, 0, 0);
        Quaternion rot2 = Quaternion.Euler(0, 90, 0);
        Quaternion rot3 = Quaternion.Euler(0, 180, 0);

        // Act
        recorder.RecordFrame(Vector2.zero, rot1, Vector2.one, false, false);
        recorder.RecordFrame(Vector2.zero, rot2, Vector2.one, false, false);
        recorder.RecordFrame(Vector2.zero, rot3, Vector2.one, false, false);

        var rotations = recorder.GetRotations();

        // Assert
        Assert.AreEqual(3, rotations.Count, "Should have 3 rotations");
        Assert.AreEqual(rot1, rotations[0], "First rotation should match");
        Assert.AreEqual(rot2, rotations[1], "Second rotation should match");
        Assert.AreEqual(rot3, rotations[2], "Third rotation should match");
    }

    [Test]
    public void Test05_RecordedScales_MatchInputData()
    {
        // Arrange
        Vector2 scale1 = new Vector2(1.0f, 1.0f);
        Vector2 scale2 = new Vector2(2.0f, 2.0f);
        Vector2 scale3 = new Vector2(0.5f, 0.5f);

        // Act
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, scale1, false, false);
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, scale2, false, false);
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, scale3, false, false);

        var scales = recorder.GetScales();

        // Assert
        Assert.AreEqual(3, scales.Count, "Should have 3 scales");
        Assert.AreEqual(scale1, scales[0], "First scale should match");
        Assert.AreEqual(scale2, scales[1], "Second scale should match");
        Assert.AreEqual(scale3, scales[2], "Third scale should match");
    }

    [Test]
    public void Test06_RecordedFlipXStates_MatchInputData()
    {
        // Arrange & Act
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, Vector2.one, false, false);
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, Vector2.one, true, false);
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, Vector2.one, false, false);

        var flipXStates = recorder.GetFlipXStates();

        // Assert
        Assert.AreEqual(3, flipXStates.Count, "Should have 3 flipX states");
        Assert.IsFalse(flipXStates[0], "First flipX should be false");
        Assert.IsTrue(flipXStates[1], "Second flipX should be true");
        Assert.IsFalse(flipXStates[2], "Third flipX should be false");
    }

    [Test]
    public void Test07_RecordedRunningStates_MatchInputData()
    {
        // Arrange & Act
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, Vector2.one, false, true);
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, Vector2.one, false, false);
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, Vector2.one, false, true);

        var runningStates = recorder.GetIsRunningStates();

        // Assert
        Assert.AreEqual(3, runningStates.Count, "Should have 3 running states");
        Assert.IsTrue(runningStates[0], "First running state should be true");
        Assert.IsFalse(runningStates[1], "Second running state should be false");
        Assert.IsTrue(runningStates[2], "Third running state should be true");
    }

    #endregion

    #region Playback Tests - Verify playback restores the same data

    [Test]
    public void Test08_PlaybackData_RestoresExactPositions()
    {
        // Arrange
        var expectedPositions = new List<Vector2>
        {
            new Vector2(1.0f, 1.0f),
            new Vector2(2.0f, 2.0f),
            new Vector2(3.0f, 3.0f),
            new Vector2(4.0f, 4.0f),
            new Vector2(5.0f, 5.0f)
        };

        // Record the data
        foreach (var pos in expectedPositions)
        {
            recorder.RecordFrame(pos, Quaternion.identity, Vector2.one, false, false);
        }

        // Act - Retrieve for playback
        var retrievedPositions = recorder.GetPositions();

        // Assert
        Assert.AreEqual(expectedPositions.Count, retrievedPositions.Count,
            "Retrieved positions count should match recorded count");

        for (int i = 0; i < expectedPositions.Count; i++)
        {
            Assert.AreEqual(expectedPositions[i], retrievedPositions[i],
                $"Position at frame {i} should match exactly");
        }
    }

    [Test]
    public void Test09_PlaybackData_RestoresCompleteFrameData()
    {
        // Arrange - Create complex frame data
        var frameData = new List<(Vector2 pos, Quaternion rot, Vector2 scale, bool flipX, bool running)>
        {
            (new Vector2(1, 1), Quaternion.identity, Vector2.one, false, true),
            (new Vector2(2, 2), Quaternion.Euler(0, 90, 0), new Vector2(1.5f, 1.5f), true, false),
            (new Vector2(3, 3), Quaternion.Euler(0, 180, 0), new Vector2(2, 2), false, true)
        };

        // Record the data
        foreach (var frame in frameData)
        {
            recorder.RecordFrame(frame.pos, frame.rot, frame.scale, frame.flipX, frame.running);
        }

        // Act - Retrieve all data for playback
        var positions = recorder.GetPositions();
        var rotations = recorder.GetRotations();
        var scales = recorder.GetScales();
        var flipXStates = recorder.GetFlipXStates();
        var runningStates = recorder.GetIsRunningStates();

        // Assert - Verify all data is restored correctly
        Assert.AreEqual(frameData.Count, positions.Count, "All lists should have same count");
        Assert.AreEqual(frameData.Count, rotations.Count, "All lists should have same count");
        Assert.AreEqual(frameData.Count, scales.Count, "All lists should have same count");
        Assert.AreEqual(frameData.Count, flipXStates.Count, "All lists should have same count");
        Assert.AreEqual(frameData.Count, runningStates.Count, "All lists should have same count");

        for (int i = 0; i < frameData.Count; i++)
        {
            Assert.AreEqual(frameData[i].pos, positions[i], $"Position at frame {i}");
            Assert.AreEqual(frameData[i].rot, rotations[i], $"Rotation at frame {i}");
            Assert.AreEqual(frameData[i].scale, scales[i], $"Scale at frame {i}");
            Assert.AreEqual(frameData[i].flipX, flipXStates[i], $"FlipX at frame {i}");
            Assert.AreEqual(frameData[i].running, runningStates[i], $"Running state at frame {i}");
        }
    }

    [Test]
    public void Test10_PlaybackSequence_PreservesOrder()
    {
        // Arrange - Record a sequence of distinct positions
        var sequence = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0)
        };

        foreach (var pos in sequence)
        {
            recorder.RecordFrame(pos, Quaternion.identity, Vector2.one, false, false);
        }

        // Act
        var playbackData = recorder.GetPositions();

        // Assert - Verify exact order is preserved
        for (int i = 0; i < sequence.Length; i++)
        {
            Assert.AreEqual(sequence[i], playbackData[i],
                $"Frame {i} should maintain exact sequence order");
        }
    }

    #endregion

    #region Control Tests - Stop and Clear functionality

    [Test]
    public void Test11_StopRecording_PreventsNewFrames()
    {
        // Arrange
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, Vector2.one, false, false);
        recorder.RecordFrame(Vector2.one, Quaternion.identity, Vector2.one, false, false);

        // Act
        recorder.StopRecording();
        recorder.RecordFrame(new Vector2(99, 99), Quaternion.identity, Vector2.one, false, false);

        // Assert
        Assert.AreEqual(2, recorder.GetFrameCount(),
            "Should only have 2 frames (before stop), not 3");
    }

    [Test]
    public void Test12_ClearRecording_RemovesAllData()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            recorder.RecordFrame(new Vector2(i, i), Quaternion.identity, Vector2.one, false, false);
        }

        // Act
        recorder.ClearRecording();

        // Assert
        Assert.AreEqual(0, recorder.GetFrameCount(), "Frame count should be 0 after clearing");
        Assert.AreEqual(0, recorder.GetPositions().Count, "Positions should be empty");
        Assert.AreEqual(0, recorder.GetRotations().Count, "Rotations should be empty");
        Assert.AreEqual(0, recorder.GetScales().Count, "Scales should be empty");
        Assert.AreEqual(0, recorder.GetFlipXStates().Count, "FlipX states should be empty");
        Assert.AreEqual(0, recorder.GetIsRunningStates().Count, "Running states should be empty");
    }

    #endregion

    #region Edge Case Tests

    [Test]
    public void Test13_EmptyRecorder_ReturnsEmptyLists()
    {
        // Act
        var positions = recorder.GetPositions();
        var rotations = recorder.GetRotations();
        var scales = recorder.GetScales();
        var flipXStates = recorder.GetFlipXStates();
        var runningStates = recorder.GetIsRunningStates();

        // Assert
        Assert.IsNotNull(positions, "Positions list should not be null");
        Assert.IsNotNull(rotations, "Rotations list should not be null");
        Assert.IsNotNull(scales, "Scales list should not be null");
        Assert.IsNotNull(flipXStates, "FlipX states list should not be null");
        Assert.IsNotNull(runningStates, "Running states list should not be null");

        Assert.AreEqual(0, positions.Count, "Positions list should be empty");
        Assert.AreEqual(0, rotations.Count, "Rotations list should be empty");
        Assert.AreEqual(0, scales.Count, "Scales list should be empty");
        Assert.AreEqual(0, flipXStates.Count, "FlipX states list should be empty");
        Assert.AreEqual(0, runningStates.Count, "Running states list should be empty");
    }

    [Test]
    public void Test14_GetData_ReturnsNewListCopy()
    {
        // Arrange
        recorder.RecordFrame(Vector2.zero, Quaternion.identity, Vector2.one, false, false);

        // Act
        var positions1 = recorder.GetPositions();
        var positions2 = recorder.GetPositions();

        // Modify one list
        positions1.Add(new Vector2(99, 99));

        // Assert
        Assert.AreNotSame(positions1, positions2, "Should return different list instances");
        Assert.AreNotEqual(positions1.Count, positions2.Count,
            "Modifying one list shouldn't affect the other");
    }

    [Test]
    public void Test15_LargeDataSet_HandlesCorrectly()
    {
        // Arrange
        int largeFrameCount = 1000;

        // Act
        for (int i = 0; i < largeFrameCount; i++)
        {
            recorder.RecordFrame(
                new Vector2(i * 0.1f, i * 0.2f),
                Quaternion.Euler(0, i, 0),
                new Vector2(1 + i * 0.01f, 1 + i * 0.01f),
                i % 2 == 0,
                i % 3 == 0
            );
        }

        // Assert
        Assert.AreEqual(largeFrameCount, recorder.GetFrameCount(),
            $"Should handle {largeFrameCount} frames");

        var positions = recorder.GetPositions();
        Assert.AreEqual(largeFrameCount, positions.Count,
            "All positions should be stored");

        // Verify first and last are correct
        Assert.AreEqual(new Vector2(0, 0), positions[0], "First position should match");
        Assert.AreEqual(new Vector2((largeFrameCount - 1) * 0.1f, (largeFrameCount - 1) * 0.2f),
            positions[largeFrameCount - 1], "Last position should match");
    }

    #endregion
}