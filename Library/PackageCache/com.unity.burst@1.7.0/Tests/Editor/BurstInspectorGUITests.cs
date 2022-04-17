using System.Collections;
using NUnit.Framework;
using Unity.Burst.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;

[TestFixture]
[UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor)]
public class BurstInspectorGUITests
{
    [UnityTest]
    public IEnumerator TestInspectorOpenDuringDomainReloadDoesNotLogErrors()
    {
        // Show Inspector window
        EditorWindow.GetWindow<BurstInspectorGUI>().Show();

        Assert.IsTrue(EditorWindow.HasOpenInstances<BurstInspectorGUI>());

        // Ask for domain reload
        EditorUtility.RequestScriptReload();

        // Wait for the domain reload to be completed
        yield return new WaitForDomainReload();

        Assert.IsTrue(EditorWindow.HasOpenInstances<BurstInspectorGUI>());

        // Hide Inspector window
        EditorWindow.GetWindow<BurstInspectorGUI>().Close();

        Assert.IsFalse(EditorWindow.HasOpenInstances<BurstInspectorGUI>());
    }

    //[UnityTest]
    public IEnumerator BranchHoverTest()
    {
        EditorWindow.GetWindow<BurstInspectorGUI>();

        // Make sure window is actually initialized before continuing.
        EditorUtility.RequestScriptReload();    // Ask for domain reload
        yield return new WaitForDomainReload(); // Wait for the domain reload to be completed


        var window = EditorWindow.GetWindow<BurstInspectorGUI>();

        // Selecting a specific assembly.
        window._treeView.TrySelectByDisplayName("BurstInspectorGUITests.MyJob - (IJob)");

        // Sending event to set the displayname, to avoid it resetting _scrollPos because of target change.
        window.SendEvent(new Event() { type = EventType.Repaint, mousePosition = new Vector2(window.position.width / 2f, window.position.height / 2f) });

        // Setting up for the test.
        // Finding an edge:
        int dstBlockIdx = -1;
        int srcBlockIdx = -1;
        int line = -1;
        for (int idx = 0; idx < window._burstDisassembler.Blocks.Count; idx++)
        {
            var block = window._burstDisassembler.Blocks[idx];
            if (block.Edges != null)
            {
                foreach (var edge in block.Edges)
                {
                    if (edge.Kind == BurstDisassembler.AsmEdgeKind.OutBound)
                    {
                        dstBlockIdx = edge.LineRef.BlockIndex;
                        line = window._textArea._blockLine[dstBlockIdx];
                        if ((dstBlockIdx == idx + 1 && edge.LineRef.LineIndex == 0)) // pointing to next line
                        {
                            continue;
                        }
                        srcBlockIdx = idx;
                        break;
                    }
                }
                if (srcBlockIdx != -1)
                {
                    break;
                }
            }
        }
        if (srcBlockIdx == -1)
        {
            window.Close();
            throw new System.Exception("No edges present in assembly for \"BurstInspectorGUITests.MyJob - (IJob)\"");
        }

        float dist = line * window._textArea.fontHeight;

        float x = (window.position.width - (window._inspectorView.width + BurstInspectorGUI._scrollbarThickness)) + window._textArea.horizontalPad - (6 + window._textArea.fontWidth);

        // setting _ScrollPos so end of arrow is at bottom of screen, to make sure there is actually room for the scrolling.
        window._scrollPos = new Vector2(0, dist - window._inspectorView.height * 0.95f);

        // Setting mousePos to bottom of inspector view.
        float topOfInspectorToBranchArrow = window._buttonOverlapInspectorView + 66.5f;//66.5f is approximately the size of space over the treeview of different jobs.

        var mousePos = new Vector2(x, topOfInspectorToBranchArrow + window._textArea.fontHeight*0.6f + window._inspectorView.height * 0.95f);



        window.SendEvent(new Event() { type = EventType.MouseUp, mousePosition = mousePos });
        var branch = window._textArea._hoveredBranch;

        // Close window to avoid it sending more events
        window.Close();

        Assert.AreNotEqual(branch, default(LongTextArea.Branch));
        Assert.AreEqual(srcBlockIdx, branch.Edge.OriginRef.BlockIndex);
        Assert.AreEqual(dstBlockIdx, branch.Edge.LineRef.BlockIndex);
    }

    [BurstCompile]
    private struct MyJob : IJob
    {
        [ReadOnly]
        public NativeArray<float> Input;

        [WriteOnly]
        public NativeArray<float> Output;

        public void Execute()
        {
            float result = 0.0f;
            for (int i = 0; i < Input.Length; i++)
            {
                result += Input[i];
            }
            Output[0] = result;
        }
    }
}
