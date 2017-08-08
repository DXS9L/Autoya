using Miyamasu;
using MarkdownSharp;
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
	test for html parser.
 */
public class HTMLParserTests : MiyamasuTestRunner {
    private HTMLParser parser;

    private InformationResourceLoader loader;

	[MSetup] public void Setup () {

		// GetTexture(url) runs only Play mode.
		if (!IsTestRunningInPlayingMode()) {
			SkipCurrentTest("Information feature should run on MainThread.");
		};

        loader = new InformationResourceLoader(Autoya.Mainthread_Commit, null, null);
        parser = new HTMLParser(loader);
	}

    [MTest] public void LoadSimpleHTML () {
        var sampleHtml = @"
<body>something</body>
        ";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 1, "too late."
        );
        
        var children = parsedRoot.GetChildren();

        Assert(children.Count == 1, "not match. children.Count:" + children.Count);
    }

    [MTest] public void LoadDepthAssetListIsDone () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/ParserTest/DepthAssetList)-->
<body>something</body>
        ";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 5, "too late."
        );

        Assert(!loader.IsLoadingDepthAssetList, "still loading.");
    }

    [MTest] public void LoadDepthAssetListWithCustomTag () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/ParserTest/DepthAssetList)-->
<customtag><customtagtext>something</customtagtext></customtag>
        ";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 5, "too late."
        );

        Assert(!loader.IsLoadingDepthAssetList, "still loading.");
    }


    // 解析した階層が想定通りかどうか

    [MTest] public void ParseSimpleHTML () {
        var sampleHtml = @"
<body>something</body>
        ";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 1, "too late."
        );
        
        var children = parsedRoot.GetChildren();

        Assert(parsedRoot.GetChildren().Count == 1, "not match.");
        Assert(parsedRoot.GetChildren()[0].parsedTag == (int)HtmlTag.body, "not match.");
        
    }

    [MTest] public void ParseCustomTag () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/ParserTest/DepthAssetList)-->
<customtag><customtagtext>something</customtagtext></customtag>
        ";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 5, "too late."
        );

        // loader contains 3 additional custom tags.
        var count = loader.GetAdditionalTagCount();
        Assert(count == 3, "not match. count:" + count);

        Assert(parsedRoot.GetChildren().Count == 1, "not match.");
        Assert(parsedRoot.GetChildren()[0].parsedTag == 33, "not match. actual:" + parsedRoot.GetChildren()[0].parsedTag);
        Assert(parsedRoot.GetChildren()[0].GetChildren()[0].parsedTag == ((int)HtmlTag._END) + 3, "not match +3. actual:" + parsedRoot.GetChildren()[0].GetChildren()[0].parsedTag);
    }

    [MTest] public void ParseCustomTagMoreDeep () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/ParserTest/DepthAssetList)-->
<customtag><customtagtext>
    <customtag2><customtagtext2>something</customtagtext2></customtag2>
</customtagtext></customtag>
        ";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 5, "too late."
        );

        // loader contains 4 additional custom tags.
        var count = loader.GetAdditionalTagCount();
        Assert(count == 6, "not match. count:" + count);

        Assert(parsedRoot.GetChildren().Count == 1, "not match a.");
        Assert(parsedRoot.GetChildren()[0].parsedTag == 33, "not match b. actual:" + parsedRoot.GetChildren()[0].parsedTag);
        Assert(parsedRoot.GetChildren()[0].GetChildren()[0].parsedTag == 38, "not match c. actual:" + parsedRoot.GetChildren()[0].GetChildren()[0].parsedTag);
        Assert(parsedRoot.GetChildren()[0].GetChildren()[0].GetChildren()[0].parsedTag == 34, "not match d. actual:" + parsedRoot.GetChildren()[0].GetChildren()[0].GetChildren()[0].parsedTag);
        Assert(parsedRoot.GetChildren()[0].GetChildren()[0].GetChildren()[0].GetChildren()[0].parsedTag == 35, "not match e. actual:" + parsedRoot.GetChildren()[0].GetChildren()[0].GetChildren()[0].GetChildren()[0].parsedTag);
    }


    [MTest] public void ParseCustomTagRecursive () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/ParserTest/DepthAssetList)-->
<customtag><customtagtext>
    something<customtag><customtagtext>else</customtagtext></customtag>other
</customtagtext></customtag>
        ";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 5, "too late."
        );

        // loader contains 2 additional custom tags.
        var count = loader.GetAdditionalTagCount();
        Assert(count == 3, "not match. count:" + count);

        Assert(parsedRoot.GetChildren().Count == 1, "not match.");
        Assert(parsedRoot.GetChildren()[0].parsedTag == 33, "not match a. actual:" + parsedRoot.GetChildren()[0].parsedTag);
        Assert(parsedRoot.GetChildren()[0].GetChildren()[0].parsedTag == ((int)HtmlTag._END) + 3, "not match b. actual:" + parsedRoot.GetChildren()[0].GetChildren()[0].parsedTag);
    }

    [MTest] public void ParseImageAsImgContent () {
        var sampleHtml = @"
<img src='https://github.com/sassembla/Autoya/blob/master/doc/scr.png?raw=true2' />";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 5, "too late."
        );

        Assert(parsedRoot.GetChildren().Count == 1, "not match.");
        Assert(parsedRoot.GetChildren()[0].parsedTag == (int)HtmlTag.img, "not match 1. actual:" + parsedRoot.GetChildren()[0].parsedTag);
        Assert(parsedRoot.GetChildren()[0].treeType == TreeType.Content_Img, "not match.");
    }

    [MTest] public void ParseCustomImgAsImgContent () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/ParserTestImgView/DepthAssetList)-->
<myimg src='https://github.com/sassembla/Autoya/blob/master/doc/scr.png?raw=true2' />";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 5, "too late."
        );

        Assert(parsedRoot.GetChildren().Count == 1, "not match.");
        Assert(parsedRoot.GetChildren()[0].treeType == TreeType.Content_Img, "not match. expected:" + TreeType.Content_Img + " actual:" + parsedRoot.GetChildren()[0].treeType);
    }

    [MTest] public void ParseCustomTextAsTextContent () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/ParserTestTextView/DepthAssetList)-->
<mytext>text</mytext>";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 5, "too late."
        );

        Assert(parsedRoot.GetChildren().Count == 1, "not match.");
        Assert(parsedRoot.GetChildren()[0].treeType == TreeType.Container, "not match. expected:" + TreeType.Container + " actual:" + parsedRoot.GetChildren()[0].treeType);
    }

    [MTest] public void ParserTestCustomLayerAndCustomContentCombination () {
        var sampleHtml = @"
<!--depth asset list url(resources://Views/ParserTestCombination/DepthAssetList)-->
<customtag><customtagtext><customtext>text</customtext></customtagtext></customtag>
<customtext>text</customtext>";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 5, "too late."
        );

        Assert(parsedRoot.GetChildren().Count == 2, "not match.");
        Assert(parsedRoot.GetChildren()[0].treeType == TreeType.CustomLayer, "not match. expected:" + TreeType.CustomLayer + " actual:" + parsedRoot.GetChildren()[0].treeType);
        Assert(parsedRoot.GetChildren()[1].treeType == TreeType.Container, "not match. expected:" + TreeType.Container + " actual:" + parsedRoot.GetChildren()[0].treeType);
    }
    
    [MTest] public void Revert () {
        var sampleHtml = @"
<body>something</body>
        ";

        ParsedTree parsedRoot = null;
        var cor = parser.ParseRoot(sampleHtml, parsed => {
            parsedRoot = parsed;
        });
        Autoya.Mainthread_Commit(cor);
        
        WaitUntil(
            () => parsedRoot != null, 1, "too late."
        );

        {
            var bodyContainer = parsedRoot.GetChildren()[0];
            
            var textChildren = bodyContainer.GetChildren();

            Assert(textChildren.Count == 1, "not match a. actual:" + textChildren.Count);
            
            var textChildrenTree = textChildren[0];
            var textPart = textChildrenTree.keyValueStore[HTMLAttribute._CONTENT] as string;
            var frontHalf = textPart.Substring(0,4);
            var backHalf = textPart.Substring(4);

            textChildrenTree.keyValueStore[HTMLAttribute._CONTENT] = frontHalf;

            var insertionTree = new InsertedTree(textChildrenTree, backHalf, textChildrenTree.parsedTag);
            insertionTree.SetParent(bodyContainer);

            // 増えてるはず
            Assert(bodyContainer.GetChildren().Count == 2, "not match b. actual:" + bodyContainer.GetChildren().Count);
        }

        parsedRoot = ParsedTree.RevertInsertedTree(parsedRoot);

        {
            var bodyContainer = parsedRoot.GetChildren()[0];
            
            var textChildren = bodyContainer.GetChildren();
            var textChildrenTree = textChildren[0];
            
            Assert(textChildren.Count == 1, "not match c. actual:" + textChildren.Count);
            Assert(textChildrenTree.keyValueStore[HTMLAttribute._CONTENT] as string == "something", "actual:" + textChildrenTree.keyValueStore[HTMLAttribute._CONTENT] as string);
        }
    }
}