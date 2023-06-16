using UnityEngine;
using VentLib.Options.Game.Interfaces;

namespace VentLib.Options.Game;

public class GameOptionTitleBuilder
{
    private GameOptionBuilder builder = new GameOptionBuilder().IsTitle(true).IsHeader(true).Value(0).Attribute("Title", true);
    private IGameOptionTab? titleTab;

    public GameOptionTitleBuilder Title(string text)
    {
        builder = builder.Name(text);
        return this;
    }

    public GameOptionTitleBuilder IsHeader(bool isHeader)
    {
        builder = builder.IsHeader(isHeader);
        return this;
    }
    
    public GameOptionTitleBuilder Color(Color color)
    {
        builder = builder.Color(color);
        return this;
    }

    public GameOptionTitleBuilder Tab(IGameOptionTab tab)
    {
        titleTab = tab;
        return this;
    }

    public GameOption Build()
    {
        if (titleTab != null) builder = builder.Tab(titleTab);
        return builder.Build();
    }
}