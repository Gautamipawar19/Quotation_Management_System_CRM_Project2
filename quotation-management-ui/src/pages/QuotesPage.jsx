import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getAllQuotes } from "../api/quoteApi";
import Loader from "../components/common/Loader";
import ErrorMessage from "../components/common/ErrorMessage";
import QuoteTable from "../components/quotes/QuoteTable";

export default function QuotesPage() {
  const [quotes, setQuotes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchQuotes();
  }, []);

  const fetchQuotes = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await getAllQuotes();
      console.log("Quotes API Response:", response);

      const quotesArray = response?.data || [];
      setQuotes(quotesArray);
    } catch (err) {
      console.log("Quotes Fetch Error:", err?.response?.data || err);
      setError("Failed to load quotes.");
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <Loader />;

  return (
    <div>
      <div className="page-header">
        <h1>Quotes</h1>
        <Link to="/quotes/create" className="primary-btn">
          Create Quote
        </Link>
      </div>

      <ErrorMessage message={error} />
      <QuoteTable quotes={quotes} />
    </div>
  );
}