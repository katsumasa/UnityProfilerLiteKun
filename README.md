# UnityProfilerLiteKun [(EnglishVer)](Documentation~/UnityProfilerLiteKun.md)

![GitHub package.json version](https://img.shields.io/github/package-json/v/katsumasa/UnityProfilerLiteKun)

<img width="800" alt="a532971f88c85a4653daae4dab0280b0" src="https://user-images.githubusercontent.com/29646672/137266796-2e436fbd-14f7-48ce-82af-32369759327b.gif">

## 概要

簡易的なUnity Profiler。

## このEditor拡張で出来ること

- UnityEditorに簡易的なプロファイリング情報を表示
- メモリが許す限り、プロファイリング情報を取得
- 取得したプロファイリング情報をCSV形式で保存
- Player側にStats風の簡易的なプロファイリング情報を表示可能

<img width="800" alt="Stats" src="https://user-images.githubusercontent.com/29646672/137267690-ed73cf86-15fd-46da-b66f-65cc6221e071.png">

## 実行確認済み環境

### 確認済みのUnityバージョン

- Unity2019.4.5f1
- Unity2020.2.2f1

### 確認済みの端末

#### Android

- Pixel4XL

## 使い方

1. [UnityProfilerLiteKun.prefab](https://github.com/katsumasa/UnityProfilerLiteKun/blob/master/Runtime/Prefabs/UnityProfilerLiteKun.prefab)をSceneに配置する。
2. アプリをビルドする（Development　Build:ON(必須)、Autoconnect Profiler:ON(推奨)
3. Window > UTJ > UnityProfilerLiteKunから専用Windowを開く
4. アプリを実行する
5. 任意のタイミングでRecordボタンを押す

以上

## API

特定のタイミング・期間で計測を行う必要がある場合、スクリプトから下記のAPIを実行してください。

- UnityProfilerLiteKun.instance.StartRecording()
- UnityProfilerLiteKun.instance.EndRecording()

## その他

パフォーマンスのチューニングを行う際、いきなりUnityProfilerをでProfilingを行うのではなく、先ずは(メモリの許す限り)Frame数に制限無くProfilingを行うことが出来る為、常にProfileを記録しつつ、パフォーマンス悪い画面を見つけたら、UnityProfilerでProfilingを行うと効率的です。
本Editor拡張は、[UnityChoseKun](https://github.com/katsumasa/UnityChoseKun)と併用して使うことを想定しています。
Statsを有効にすると、Frame数が画面に表示されるので、UnityChoseKunで画面を録画しておくと、どの画面でパフォーマンスが悪くなるかを発見しやすくなります。

要望、問題等はIssuesからご連絡下さい。
