# JsonTranslator-2
jsonで翻訳管理してる人向けの翻訳補助アプリ。前のやつよりちょっと良くなったかも。

# これが使い方だと思う
はじめに、Lang Dataの下にあるボックスに原文ファイルを貼り付け、Reset and Loadをクリックしてください。
Translationsから翻訳したいアイテムを選び、"Original Text"の翻訳を"Translated Text"に入力してApplyをクリックします。 (またはCtrl + Enter)

翻訳が終わったら、Lang Dataのテキストを全てコピーして別のファイルに保存します。

## 注意点(?)
キーはStringのみ、
値はString, List, Dictionaryをサポートしています。
解析が曖昧になるため、キーに"/"を含めることはできません。
