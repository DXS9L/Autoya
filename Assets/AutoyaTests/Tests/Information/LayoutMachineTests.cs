using Miyamasu;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using AutoyaFramework.Information;
using System.Collections;
using AutoyaFramework;

/**
	test for layoutMachine.
 */
public class LayoutMachineTests : MiyamasuTestRunner {
    private HTMLParser parser;

    private ResourceLoader loader;

    private UUebView executor;

    private void ShowLayoutRecursive (TagTree tree) {
        Debug.Log("tree:" + loader.GetTagFromValue(tree.tagValue) + " offsetX:" + tree.offsetX + " offsetY:" + tree.offsetY + " width:" + tree.viewWidth + " height:" + tree.viewHeight);
        foreach (var child in tree.GetChildren()) {
            ShowLayoutRecursive(child);
        }
    }

	[MSetup] public void Setup () {

		// GetTexture(url) runs only Play mode.
		if (!IsTestRunningInPlayingMode()) {
			SkipCurrentTest("Information feature should run on MainThread.");
		};

        RunOnMainThread(
            () => {
                executor = new GameObject("layoutMachineTest").AddComponent<UUebView>();
                loader = new ResourceLoader(executor.CoroutineExecutor);
            }
        );

        parser = new HTMLParser(loader);
	}

    [MTeardown] public void Teardown () {
        RunOnMainThread(
            () => {
                GameObject.DestroyImmediate(executor);
                GameObject.DestroyImmediate(loader.cacheBox);
            }
        );
    }

    private ParsedTree CreateTagTree (string sampleHtml) {
        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(
            sampleHtml, 
            parsed => {
                parsedRoot = parsed;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));
        
        WaitUntil(
            () => parsedRoot != null, 1, "too late."
        );

        return parsedRoot;
    }

    [MTest] public void LayoutHTML () {
        var sample = @"
<body>something</body>";
        var tree = CreateTagTree(sample);
        
        

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));

        WaitUntil(
            () => layouted != null, 5, "timeout."
        );

    }

    [MTest] public void LayoutHTMLHasValidView () {
        var sample = @"
<body>something</body>";
        var tree = CreateTagTree(sample);
        
        

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                Assert(layoutedTree.viewHeight == 16, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));
        WaitUntil(
            () => done, 5, "timeout."
        );

    }

    [MTest] public void LayoutHTMLWithSmallTextHasValidView () {
        var sample = @"
<body>over 100px string should be multi lined text with good separation. need some length.</body>";
        var tree = CreateTagTree(sample);
        
        

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                Assert(layoutedTree.viewHeight == 112, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));

        WaitUntil(
            () => done, 5, "timeout."
        );
        
    }

    [MTest] public void LayoutHTMLWithImage () {
        var sample = @"
<body><img src='https://dummyimage.com/100.png/09f/fff'/></body>";
        var tree = CreateTagTree(sample);
        
        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                Assert(layoutedTree.viewHeight == 100, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }

    [MTest] public void LayoutHTMLWithSmallImage () {
        var sample = @"
<body><img src='https://dummyimage.com/10.png/09f/fff'/></body>";
        var tree = CreateTagTree(sample);
        
        

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                Assert(layoutedTree.viewHeight == 10, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));

        WaitUntil(
            () => done, 5, "timeout."
        );

    }

    [MTest] public void LayoutHTMLWithSmallImageAndText () {
        var sample = @"
<body><img src='https://dummyimage.com/10.png/09f/fff'/>text</body>";
        var tree = CreateTagTree(sample);
        
        

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                Assert(layoutedTree.viewHeight == 16, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }

    [MTest] public void LayoutHTMLWithSmallImageAndSmallText () {
        var sample = @"
<body><img src='https://dummyimage.com/10.png/09f/fff'/>over 100px string should be multi lined text with good separation. need some length.</body>";
        var tree = CreateTagTree(sample);

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                Assert(layoutedTree.viewHeight == 112, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }
    

    [MTest] public void LayoutHTMLWithWideImageAndText () {
        var sample = @"
<body><img src='https://dummyimage.com/97x10/000/fff'/>something</body>";
        var tree = CreateTagTree(sample);

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                // ShowLayoutRecursive(layoutedTree);
                done = true;
                Assert(layoutedTree.viewHeight == 26, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }
    [MTest] public void LayoutHTMLWithTextAndWideImage () {
        var sample = @"
<body>something<img src='https://dummyimage.com/100x10/000/fff'/></body>";
        var tree = CreateTagTree(sample);

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                // ShowLayoutRecursive(layoutedTree);
                done = true;
                Assert(layoutedTree.viewHeight == 16, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }


    [MTest] public void LayoutHTMLWithTextAndWideImageAndText () {
        var sample = @"
<body>something<img src='https://dummyimage.com/100x10/000/fff'/>else</body>";
        var tree = CreateTagTree(sample);
        
        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                // ShowLayoutRecursive(layoutedTree);
                done = true;
                Assert(layoutedTree.viewHeight == 16+16, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }

    [MTest] public void LayoutHTMLWithTextAndWideImageAndTextAndWideImageAndText () {
        var sample = @"
<body>something<img src='https://dummyimage.com/100x10/000/fff'/>else<img src='https://dummyimage.com/100x20/000/fff'/>other</body>";
        var tree = CreateTagTree(sample);
        
        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                // ShowLayoutRecursive(layoutedTree);
                done = true;
                Assert(layoutedTree.viewHeight == 16+16+16, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }

    [MTest] public void LayoutHTMLWithWideImageAndTextAndWideImageAndText () {
        var sample = @"
<body><img src='https://dummyimage.com/100x10/000/fff'/>else<img src='https://dummyimage.com/100x20/000/fff'/>other</body>";
        var tree = CreateTagTree(sample);
        
        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                // ShowLayoutRecursive(layoutedTree);
                done = true;
                Assert(layoutedTree.viewHeight == 10+16+16, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }


    [MTest] public void LayoutHTMLWithTextAndSmallImage () {
        var sample = @"
<body>something<img src='https://dummyimage.com/10x10/000/fff'/></body>";
        var tree = CreateTagTree(sample);

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                // ShowLayoutRecursive(layoutedTree);
                done = true;
                Assert(layoutedTree.viewHeight == 16, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }


    [MTest] public void LayoutHTMLWithTextAndSmallImageAndText () {
        var sample = @"
<body>something<img src='https://dummyimage.com/10x10/000/fff'/>b!</body>";
        var tree = CreateTagTree(sample);
        
        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                // ShowLayoutRecursive(layoutedTree);
                done = true;
                Assert(layoutedTree.viewHeight == 16, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }

    [MTest] public void LayoutHTMLWithTextAndSmallImageAndTextAndWideImageAndText () {
        var sample = @"
<body>something<img src='https://dummyimage.com/10x10/000/fff'/>else<img src='https://dummyimage.com/100x10/000/fff'/>other</body>";
        var tree = CreateTagTree(sample);
        
        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                // ShowLayoutRecursive(layoutedTree);
                done = true;
                Assert(layoutedTree.viewHeight == 16+16+16, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }

    [MTest] public void LayoutHTMLWithSmallImageAndTextAndSmallImageAndText () {
        var sample = @"
<body><img src='https://dummyimage.com/10x10/000/fff'/>else<img src='https://dummyimage.com/10x20/000/fff'/>other</body>";
        var tree = CreateTagTree(sample);
        
        var done = false;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                // ShowLayoutRecursive(layoutedTree);
                done = true;
                Assert(layoutedTree.viewHeight == 20, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );
    }


    [MTest] public void LoadHTMLWithCustomTagLink () {
        var sample = @"
<!--depth asset list url(resources://Views/LayoutHTMLWithCustomTag/DepthAssetList)-->
        ";
        var tree = CreateTagTree(sample);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );
    }

    [MTest] public void LayoutHTMLWithCustomTag () {
        var sample = @"
<!--depth asset list url(resources://Views/LayoutHTMLWithCustomTag/DepthAssetList)-->
<body>
<customtag><custombg><textbg><customtext>something</customtext></textbg></custombg></customtag>
else
<customimg src='https://dummyimage.com/10x20/000/fff'/>
</body>
        ";
        var tree = CreateTagTree(sample);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );
    }

    [MTest] public void LayoutHTMLWithCustomTagSmallText () {
        var sample = @"
<!--depth asset list url(resources://Views/LayoutHTMLWithCustomTag/DepthAssetList)-->
<body>
<customtag><custombg><textbg><customtext>
something you need is not time, money, but do things fast.
</customtext></textbg></custombg></customtag>
else
</body>";
        var tree = CreateTagTree(sample);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );
    }

    [MTest] public void LayoutHTMLWithCustomTagLargeText () {
        var sample = @"
<!--depth asset list url(resources://Views/LayoutHTMLWithCustomTag/DepthAssetList)-->
<body>
<customtag><custombg><textbg><customtext>
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
</customtext></textbg></custombg></customtag>
else
</body>";
        var tree = CreateTagTree(sample);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
                
                var localTree = layouted;
                while (true) {
                    if (0 < localTree.GetChildren().Count) {
                        localTree = localTree.GetChildren()[localTree.GetChildren().Count-1];
                        if (localTree.offsetY != 0) {
                            Assert(localTree.offsetY.Equals(755.6f), "not match, offsetY:" + localTree.offsetY);
                        }
                    } else {
                        break;
                    }
                }
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );
    }


    [MTest] public void Re_LayoutHTMLWithSmallImageAndSmallText () {
        var sample = @"
<body><img src='https://dummyimage.com/10.png/09f/fff'/>over 100px string should be multi lined text with good separation. need some length.</body>";
        var tree = CreateTagTree(sample);

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader      
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                Assert(layoutedTree.viewHeight == 112, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );

        /*
            re-layout.
         */
        var done2 = false;
        var layoutMachine2 = new LayoutMachine(
            loader
        );

        var cor2 = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done2 = true;
                Assert(layoutedTree.viewHeight == 112, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor2));


        WaitUntil(
            () => done2, 5, "timeout."
        );
    }

    [MTest] public void RevertLayoutHTMLWithSmallImageAndSmallText () {
        var sample = @"
<body><img src='https://dummyimage.com/10.png/09f/fff'/>over 100px string should be multi lined text with good separation. need some length.</body>";
        
        var tree = CreateTagTree(sample);

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader      
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                Assert(layoutedTree.viewHeight == 112, "not match.");
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => done, 5, "timeout."
        );

        TagTree.CorrectTrees(tree);

        /*
            revert-layout.
         */
        var done2 = false;
        
        var cor2 = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done2 = true;
                Assert(layoutedTree.viewHeight == 112, "not match. actual:" + layoutedTree.viewHeight);
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor2));


        WaitUntil(
            () => done2, 5, "timeout."
        );
    }

    [MTest] public void Order () {
        var sample = @"
<body>something1.<img src='https://dummyimage.com/100.png/09f/fff'/></body>";
        var tree = CreateTagTree(sample);

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader      
        );

        TagTree currentLayoutedTree = null;

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                currentLayoutedTree = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));
        
        WaitUntil(
            () => done, 5, "too late."
        );

        Assert(
            currentLayoutedTree/*root*/.GetChildren()[0]/*body*/.GetChildren()[0]/*text of body*/.treeType == TreeType.Content_Text, "not match, type:" + currentLayoutedTree/*root*/.GetChildren()[0]/*body*/.GetChildren()[0]/*text of body*/.treeType
        );

        Assert(
            currentLayoutedTree/*root*/.GetChildren()[0]/*body*/.GetChildren()[1]/*img*/.treeType == TreeType.Content_Img, "not match, type:" + currentLayoutedTree/*root*/.GetChildren()[0]/*body*/.GetChildren()[1]/*img*/.treeType
        );

    }

    [MTest] public void Position () {
        var sample = @"
<body>something1.<img src='https://dummyimage.com/100.png/09f/fff'/></body>";
        var tree = CreateTagTree(sample);

        var done = false;
        var layoutMachine = new LayoutMachine(
            loader      
        );

        TagTree currentLayoutedTree = null;

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                done = true;
                currentLayoutedTree = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));
        
        WaitUntil(
            () => done, 5, "too late."
        );

        Assert(
            currentLayoutedTree/*root*/.GetChildren()[0]/*body*/.GetChildren()[0]/*text of body*/.offsetY == 6f, "not match, offsetY:" + currentLayoutedTree/*root*/.GetChildren()[0]/*body*/.GetChildren()[0]/*text of body*/.offsetY
        );

        Assert(
            currentLayoutedTree/*root*/.GetChildren()[0]/*body*/.GetChildren()[1]/*img*/.offsetY == 0, "not match, offsetY:" + currentLayoutedTree/*root*/.GetChildren()[0]/*body*/.GetChildren()[1]/*img*/.offsetY
        );

    }

    [MTest] public void MultipleBoxConstraints () {
        var sample = @"
<!--depth asset list url(resources://Views/MultipleBoxConstraints/DepthAssetList)-->
<itemlayout>
<topleft>
    <img src='https://dummyimage.com/100.png/09f/fff'/>
</topleft>
<topright>
    <img src='https://dummyimage.com/100.png/08f/fff'/>
</topright>
<content><p>something!</p></content>
<bottom>
    <img src='https://dummyimage.com/100.png/07f/fff'/>
</bottom>
</itemlayout>";
        var tree = CreateTagTree(sample);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
                var itemLayout = layoutedTree.GetChildren()[0];
                var topleft = itemLayout.GetChildren()[0];
                var topright = itemLayout.GetChildren()[1];
                Assert(topleft.offsetY == 0, "not match, topleft.offsetY:" + topleft.offsetY);
                Assert(topright.offsetY == 0, "not match, topright.offsetY:" + topright.offsetY);
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );
    }

    [MTest] public void LayoutErrorAtDirectContentUnderLayer () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/MultipleBoxConstraints/DepthAssetList)-->
<itemlayout>
<topleft>
    <img src='https://dummyimage.com/100.png/09f/fff'/>
</topleft>
<topright>
    <img src='https://dummyimage.com/100.png/08f/fff'/>
</topright>
<content>something!</content>
<bottom>
    <img src='https://dummyimage.com/100.png/07f/fff'/>
</bottom>
</itemlayout>";
        var tree = CreateTagTree(sampleHtml);

        Assert(tree.errors[0].code == (int)ParseErrors.CANNOT_CONTAIN_TEXT_IN_BOX_DIRECTLY, "not match.");
    }

    [MTest] public void LayoutHTMLWithCustomTagMultiple () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/LayoutHTMLWithCustomTag/DepthAssetList)-->
<customtag><custombg><textbg><customtext>something1</customtext></textbg></custombg></customtag>
<customtag><custombg><textbg><customtext>something2</customtext></textbg></custombg></customtag>";
        var tree = CreateTagTree(sampleHtml);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );

        // ShowLayoutRecursive(layouted);

        Assert(0 < layouted.GetChildren().Count, "not match, actual:" + layouted.GetChildren().Count);
        Assert(layouted.GetChildren()[0].offsetY == 0, "not match of 1. actual:" + layouted.GetChildren()[0].offsetY);
        Assert(layouted.GetChildren()[1].offsetY == 60.7f, "not match of 2. actual:" + layouted.GetChildren()[1].offsetY);
    }

    [MTest] public void LayoutHTMLWithCustomTagMultipleInBody () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/LayoutHTMLWithCustomTag/DepthAssetList)-->
<body>
<customtag><custombg><textbg><customtext>something1</customtext></textbg></custombg></customtag>
<customtag><custombg><textbg><customtext>something2</customtext></textbg></custombg></customtag>
</body>";
        var tree = CreateTagTree(sampleHtml);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );

        // ShowLayoutRecursive(layouted);

        Assert(0 < layouted.GetChildren().Count, "not match, actual:" + layouted.GetChildren().Count);
        Assert(layouted.GetChildren()[0].GetChildren()[0].offsetY == 0, "not match of 1. actual:" + layouted.GetChildren()[0].GetChildren()[0].offsetY);
        Assert(layouted.GetChildren()[0].GetChildren()[1].offsetY == 60.7f, "not match of 2. actual:" + layouted.GetChildren()[0].GetChildren()[1].offsetY);
    }

    [MTest] public void SampleView2_HiddenBreakView () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/MyInfoView/DepthAssetList)-->
<body>
    <bg>
    	<titlebox>
    		<titletext>レモン一個ぶんのビタミンC</titletext>
    	</titlebox>
    	<newbadge></newbadge>
    	<textbg>
    		<textbox>
	    		<updatetext>koko ni nihongo ga iikanji ni hairu. good thing. long text will make large window. like this.</updatetext>
	    		<!-- hiddenがあると、コンテンツが出ないみたいなのがある。連続するのがいけないのかな。 -->
	    		<updatetext hidden='true' listen='readmore'>omake!</updatetext>
	    	</textbox>
	    </textbg>
    </bg>
</body>";
        var tree = CreateTagTree(sampleHtml);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );

        Debug.LogError("hiddenを計算できてなさそう");
    }

    [MTest] public void SampleView2_NoButtonFound () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/MyInfoView/DepthAssetList)-->
<body>
    <bg>
    	<!-- 単純にボタンとして見なされてない感がある。 -->
    	<showbutton button='true' id='readmore'/>
    </bg>
</body>";
        var tree = CreateTagTree(sampleHtml);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );

        Debug.LogError("ボタンが出ない(treeTypeが違う？");
    }

    [MTest] public void SampleView2 () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/MyInfoView/DepthAssetList)-->
<body>
    <bg>
    	<titlebox>
    		<titletext>レモン一個ぶんのビタミンC</titletext>
    	</titlebox>
    	<newbadge></newbadge>
    	<textbg>
    		<textbox>
	    		<updatetext>1st line.</updatetext>
	    	</textbox>
            <textbox>
	    		<updatetext>2nd line.</updatetext>
	    	</textbox>
	    </textbg>
    </bg>
</body>";
        var tree = CreateTagTree(sampleHtml);

        TagTree layouted = null;
        var layoutMachine = new LayoutMachine(
            loader
        );

        var cor = layoutMachine.Layout(
            tree,
            new Vector2(100,100),
            layoutedTree => {
                layouted = layoutedTree;
            }
        );

        RunOnMainThread(() => executor.CoroutineExecutor(cor));


        WaitUntil(
            () => layouted != null, 5, "timeout."
        );
    }
}