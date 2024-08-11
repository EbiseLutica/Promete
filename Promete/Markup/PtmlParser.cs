﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Promete.Markup;

public static class PtmlParser
{
	public static (string plainText, IReadOnlyList<PtmlDecoration> decorations) Parse(string ptml, bool throwsIfError = false)
	{
		// プレーンテキストの部分を格納。最終的にreturnする。
		var plainTextBuilder = new StringBuilder();
		// 解析したタグを格納。最終的にreturnする
		var decorations = new List<PtmlDecoration>();
		// 解析した開始タグを格納。Peekすることで終了タグを解析し、終了タグが一致したらPopしてdecorationsへ
		var decorationStack = new Stack<PtmlDecoration>();
		// タグ解析時点のプレーンテキスト位置を格納。
		var rangeStartStack = new Stack<int>();
		// タグ名を一時保管する。タグ記法が終わったらclear
		var tagNameBuilder = new StringBuilder();
		// 属性を一時保管する。タグ記法が終わったらclear
		var attributeBuilder = new StringBuilder();

		var state = State.PlainText;
		var i = -1;

		try
		{
			foreach (var c in ptml)
			{
				i++;
				var tagName = tagNameBuilder.ToString();
				switch (state)
				{
					case State.PlainText:
						if (c == '<')
						{
							state = State.StartTagName;
							rangeStartStack.Push(plainTextBuilder.Length);
							continue;
						}

						plainTextBuilder.Append(c);
						break;

					case State.StartTagName:
						if (c == '/')
						{
							if (tagNameBuilder.Length > 0)
								throw new PtmlParserException("Invalid token /. Expected tag name.", i);

							// 終了タグの場合、先にpushしたrangeStartを破棄する
							_ = rangeStartStack.Pop();
							state = State.EndTagName;
							continue;
						}

						if (c == '=')
						{
							if (tagNameBuilder.Length == 0)
								throw new PtmlParserException("Invalid token =. Expected tag name.", i);
							state = State.Attribute;
							continue;
						}

						if (c == '>')
						{
							if (tagNameBuilder.Length == 0)
								throw new PtmlParserException("Invalid token >. Expected tag name.", i);

							decorationStack.Push(new PtmlDecoration(0, 0, tagName, attributeBuilder.ToString()));
							tagNameBuilder.Clear();
							attributeBuilder.Clear();
							state = State.PlainText;
							continue;
						}

						if (!char.IsLetterOrDigit(c))
							throw new PtmlParserException($"Token {c} cannot use as tag name.", i);
						tagNameBuilder.Append(c);
						break;
					case State.Attribute:
						if (c == '>')
						{
							if (attributeBuilder.Length == 0)
								throw new PtmlParserException("Invalid token >. Expected attribute.", i);
							decorationStack.Push(new PtmlDecoration(0, 0, tagName, attributeBuilder.ToString()));
							tagNameBuilder.Clear();
							attributeBuilder.Clear();
							state = State.PlainText;
							continue;
						}

						attributeBuilder.Append(c);
						break;
					case State.EndTagName:
						if (c == '>')
						{
							if (tagNameBuilder.Length == 0)
								throw new PtmlParserException("Invalid token >. Expected tag name.", i);
							if (!decorationStack.TryPop(out var startTag) ||
							    !startTag.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase))
								throw new PtmlParserException($"End tag \"{tagName}\" does not match to \"{startTag.TagName}\"", i);
							startTag.Start = rangeStartStack.Pop();
							startTag.End = plainTextBuilder.Length;
							decorations.Add(startTag);
							tagNameBuilder.Clear();
							state = State.PlainText;
							continue;
						}

						if (!char.IsLetterOrDigit(c))
							throw new PtmlParserException($"Token {c} cannot use as tag name.", i);
						tagNameBuilder.Append(c);
						break;
					default:
						throw new InvalidOperationException("Invalid State: " + state);
				}
			}
		}
		catch (PtmlParserException)
		{
			if (throwsIfError) throw;
			return (ptml, []);
		}

		if (state != State.PlainText)
		{
			throw new PtmlParserException($"Unexpected end of text when state is {state}.", i);
		}
		if (decorationStack.TryPeek(out var t))
		{
			throw new PtmlParserException($"Unexpected end of text. Start tag {t.TagName} is not closed.", i);
		}
		return (plainTextBuilder.ToString(), decorations.AsReadOnly());
	}

	private enum State
	{
		PlainText,
		StartTagName,
		Attribute,
		EndTagName,
	}
}
