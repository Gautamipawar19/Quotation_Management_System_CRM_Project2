import { render, screen } from "@testing-library/react";
import { BrowserRouter } from "react-router-dom";
import QuotesPage from "../pages/QuotesPage";
import { vi } from "vitest";

vi.mock("../api/quoteApi", () => ({
  getAllQuotes: vi.fn(() => Promise.resolve([])),
}));

describe("QuotesPage", () => {
  test("renders quotes heading", async () => {
    render(
      <BrowserRouter>
        <QuotesPage />
      </BrowserRouter>
    );

    expect(await screen.findByText("Quotes")).toBeInTheDocument();
  });
});