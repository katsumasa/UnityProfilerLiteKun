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

- Unity2019.4.40f1
- Unity2020.2.2f1

### 確認済みの端末

#### Android

- Pixel4XL
- Pixel6Pro

## セットアップ

UnityProfilerLiteKunはGitHubで管理されており、下記の３種類のセットアップ方法がありますが、UnityEditorのPackageManagerから取得するのがお勧めです。

### コンソールから取得する場合

コンソールを開き、以下のコマンドを実行します

```:
git clone https://github.com/katsumasa/UnityProfilerLiteKun.git
```

### GitHubから直接取得する

1. WebブラウザーでUnityProfilerLiteKunのWebページを開く
2. 画面右上の緑色のCodeと記載されているプルダウンメニューからDownload ZIPを選択する

![image](https://user-images.githubusercontent.com/29646672/186292725-d81222f9-6a5d-4474-8446-78caa926364d.png)


### PackageManagerから取得する

1. UnityEditorからWindow > Package ManagerでPackage Managerを開く
2. Package Manager左上の+のプルダウンメニューからAdd package form git URL...を選択する
3. ダイアログへ`https://github.com/katsumasa/UnityProfilerLiteKun.git`を設定し、Addボタンを押す

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
UnityProfilerLiteKunは、[UnityChoseKun](https://github.com/katsumasa/UnityChoseKun)や[UnityPlayerView](https://github.com/katsumasa/UnityPlayerView)と併用して使うことを想定しています。
Statsを有効にすると、Frame数が画面に表示されるので、[UnityPlayerView](https://github.com/katsumasa/UnityPlayerView)で画面を録画しておくと、どの画面でパフォーマンスが悪くなるかを発見しやすくなります。

要望、問題等は[Issues](https://github.com/katsumasa/UnityProfilerLiteKun/issues)からご連絡下さい。
