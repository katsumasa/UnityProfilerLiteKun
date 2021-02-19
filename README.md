# UnityProfilerLiteKun

![Demo](Docs/images/UnityProfilerLiteKunDemo.gif)

## 概要

簡易的なUnity Profiler。

## このEditor拡張で出来ること

- UnityEditorに簡易的なプロファイリング情報を表示
- メモリが許す限り、プロファイリング情報を取得
- 取得したプロファイリング情報をCSV形式で保存
- Player側にStats風の簡易的なプロファイリング情報を表示可能
  ![Stats](Docs/images/device-2021-02-19-133127.png)

## 実行確認済み環境

### 確認済みのUnityバージョン

- Unity2019.4.5f1
- Unity2020.2.2f1

### 確認済みの端末

#### Android

- Pixel4XL

## その他

パフォーマンスのチューニングを行う際、いきなりUnityProfilerをでProfilingを行うのではなく、先ずは(メモリの許す限り)Frame数に制限無くProfilingを行うことが出来る為、常にProfileを記録しつつ、パフォーマンス悪い画面を見つけたら、UnityProfilerでProfilingを行うと効率的です。
本Editor拡張は、[UnityChoseKun](https://github.com/katsumasa/UnityChoseKun)と併用して使うことを想定しています。
Statsを有効にすると、Frame数が画面に表示されるので、UnityChoseKunで画面を録画しておくと、どの画面でパフォーマンスが悪くなるかを発見しやすくなります。

要望、問題等はIssuesからご連絡下さい。
