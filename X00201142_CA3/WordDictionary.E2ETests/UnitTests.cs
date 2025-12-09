using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.Playwright.Assertions;
public class BasicTests : IAsyncLifetime
{
    private IPlaywright _playwright;
    private IBrowser _browser;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
    }

    public async Task DisposeAsync()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task HomePage_ShouldHaveTitle()
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5243/");

        var title = await page.TitleAsync();
        Assert.Equal("X00201142_CA3", title);

        var total = page.GetByText("Total: 30");
        await total.WaitForAsync();
        Assert.True(await total.IsVisibleAsync());

        var wordHeader = page.GetByRole(AriaRole.Columnheader, new() { Name = "Word" });
        Assert.True(await wordHeader.IsVisibleAsync());
        var categoryHeader = page.GetByRole(AriaRole.Columnheader, new() { Name = "Category" });
        Assert.True(await categoryHeader.IsVisibleAsync());
        var hintHeader = page.GetByRole(AriaRole.Columnheader, new() { Name = "Hint" });
        Assert.True(await hintHeader.IsVisibleAsync());
        var lettersHeader = page.GetByRole(AriaRole.Columnheader, new() { Name = "Letters" });
        Assert.True(await lettersHeader.IsVisibleAsync());
        var syllablesHeader = page.GetByRole(AriaRole.Columnheader, new() { Name = "Syllables" });
        Assert.True(await syllablesHeader.IsVisibleAsync());

    }
    [Fact]
    public async Task SearchbarTest()
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5243/");

        var title = await page.TitleAsync();
        Assert.Equal("X00201142_CA3", title);

        await page.GetByRole(AriaRole.Textbox, new() { Name = "Search word, hint, category..." }).ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Search word, hint, category..." }).FillAsync("bear");
        await page.GetByRole(AriaRole.Cell, new() { Name = "Large brown predator" }).ClickAsync();
    }

    [Fact]
    public async Task SortByTest()
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5243/");

        var title = await page.TitleAsync();
        Assert.Equal("X00201142_CA3", title);

        await page.GetByRole(AriaRole.Columnheader, new() { Name = "Word ↑" }).ClickAsync();
        await page.GetByText("turtle", new() { Exact = true }).ClickAsync();
        await page.GetByRole(AriaRole.Columnheader, new() { Name = "Letters" }).ClickAsync();
        await page.GetByRole(AriaRole.Cell, new() { Name = "3" }).First.ClickAsync();
    }

    [Fact]
    public async Task PageTests()
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5243/");

        var title = await page.TitleAsync();
        Assert.Equal("X00201142_CA3", title);

        await page.GetByText("Page 1 of").ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
        await page.GetByText("Page 2 of").ClickAsync();
        await page.GetByRole(AriaRole.Spinbutton).ClickAsync();
        await page.GetByRole(AriaRole.Spinbutton).FillAsync("3");
        await page.GetByRole(AriaRole.Spinbutton).PressAsync("Enter");
        await page.GetByText("Page 3 of").ClickAsync();
    }

    [Fact]
    public async Task GuessGameTest()
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5243/guess-game");

        // await Expect(page.GetByRole(AriaRole.Paragraph)).ToContainTextAsync("Attempts Left: 6");
        var attemptsParagraph = page.GetByRole(AriaRole.Paragraph);

        // Wait until it contains "Attempts Left: 6"
        await attemptsParagraph.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Expect(attemptsParagraph).ToHaveTextAsync("Attempts Left: 6", new() { Timeout = 10000 });

        await page.GetByRole(AriaRole.Textbox).FillAsync("abc");
        await page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        await Expect(attemptsParagraph).ToHaveTextAsync("Attempts Left: 5", new() { Timeout = 10000 });

        await page.GetByRole(AriaRole.Textbox).FillAsync("abc");
        await page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        await Expect(attemptsParagraph).ToHaveTextAsync("Attempts Left: 4", new() { Timeout = 10000 });

        await page.GetByRole(AriaRole.Textbox).FillAsync("abc");
        await page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        await Expect(attemptsParagraph).ToHaveTextAsync("Attempts Left: 3", new() { Timeout = 10000 });

        await page.GetByRole(AriaRole.Textbox).FillAsync("abc");
        await page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        await Expect(attemptsParagraph).ToHaveTextAsync("Attempts Left: 2", new() { Timeout = 10000 });

        await page.GetByRole(AriaRole.Textbox).FillAsync("abc");
        await page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        await Expect(attemptsParagraph).ToHaveTextAsync("Attempts Left: 1", new() { Timeout = 10000 });

        await page.GetByRole(AriaRole.Textbox).FillAsync("abc");
        await page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

        var lostMessage = page.GetByText("Sorry, You lost! The word was");
        await Expect(lostMessage).ToBeVisibleAsync(new() { Timeout = 10000 });
    }

    [Fact]
    public async Task GuessGameGiveUpTest()
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5243/guess-game");

        await page.GetByRole(AriaRole.Button, new() { Name = "Give Up" }).ClickAsync();

        var lostMessage = page.GetByText("Sorry, You lost! The word was");
        await Expect(lostMessage).ToBeVisibleAsync(new() { Timeout = 10000 });
    }

}
