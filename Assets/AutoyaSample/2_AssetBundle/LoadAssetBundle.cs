﻿using System;
using System.Collections;
using AutoyaFramework;
using AutoyaFramework.AssetBundles;
using UnityEngine;

public class LoadAssetBundle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.LogWarning("このサンプルは改修後動かしてない。サンプルとして正しいかチェックが必要。");
		/*
			first, Autoya manages whole assetBundle information as the "AssetBundleList".
			the structure of assetBundleList is like below.

			latest file is located at https://raw.githubusercontent.com/sassembla/Autoya/master/AssetBundles/StandaloneOSXIntel64/1.0.0/AssetBundles.StandaloneOSXIntel64_1_0_0.json
		 */
		var assetBundleListDemo = new AssetBundleList(
			"Mac",
			"1.0.0", 
			new AssetBundleInfo[]{
				// pngが一枚入ったAssetBundle
				new AssetBundleInfo(
					"bundlename", 
					new string[]{"Assets/AutoyaTests/Runtime/AssetBundles/TestResources/textureName.png"}, 
					new string[0], 
					621985162,
					"578b73927bc11f6e80072caa17983776",
					100
				),
				// 他のAssetBundleへの依存があるAssetBundle
				new AssetBundleInfo(
					"dependsbundlename", 
					new string[]{"Assets/AutoyaTests/Runtime/AssetBundles/TestResources/textureName1.prefab"}, 
					new string[]{"bundlename"}, 
					2389728195,
					"1a3bdb638b301fd91fc5569e016604ad",
					100
				),
				// もう一つ、他のAssetBundleへの依存があるAssetBundle
				new AssetBundleInfo(
					"dependsbundlename2", 
					new string[]{"Assets/AutoyaTests/Runtime/AssetBundles/TestResources/textureName2.prefab"}, 
					new string[]{"bundlename"}, 
					1194278944,
					"b24db843879f6f82d9bee95e15559003",
					100
				),
				// nestedprefab -> dependsbundlename -> bundlename
				new AssetBundleInfo(
					"nestedprefab", 
					new string[]{"Assets/AutoyaTests/Runtime/AssetBundles/TestResources/nestedPrefab.prefab"}, 
					new string[]{"dependsbundlename"}, 
					779842307,
					"30b17595dd7be703c2b04a6e4c3830ff",
					100
				),
				
			}
		);

		// you can get that assetBundleList sample from web.
		var assetBundleListPath = "https://raw.githubusercontent.com/sassembla/Autoya/master/AssetBundles/StandaloneOSXIntel64/1.0.0/AssetBundles.StandaloneOSXIntel64_1_0_0.json";
		Autoya.Debug_AssetBundle_DownloadAssetBundleListFromUrl(
			assetBundleListPath, 
			() => {
				Debug.Log("assetBundleList download succeeded.");
				
				/*
					then, you can load asset from web.
						
					assetBundleList has the information which asset is contained by specific assetBundle.
						(asset <-containes-- assetBundle <-info contains-- assetBundleList)

					you can load asset from web after downloading the assetBundleList.

					the downloaded assetBundleList is stored in device. you can set the location and the way of read/write the list via OverridePoint.cs.
				*/

				/*
					load asset from web or cache.
					automatically download bundle then load asset.
				*/
				Autoya.AssetBundle_LoadAsset<GameObject>(
					"Assets/AutoyaTests/RuntimeData/AssetBundles/TestResources/textureName1.prefab",
					(assetName, prefab) => {
						Debug.Log("asset:" + assetName + " is successfully loaded as:" + prefab);

						// instantiate asset.
						Instantiate(prefab);
					},
					(assetName, err, reason, status) => {
						Debug.LogError("failed to load assetName:" + assetName + " err:" + err + " reason:" + reason);
					}
				);
			},
			(code, reason, autoyaStatus) => {
				Debug.LogError("failed to download assetBundleList from url:" + assetBundleListPath);
			}
		);
	}

	void OnApplicationQuit () {
		Autoya.AssetBundle_DeleteAllStorageCache();
	}
	
}
