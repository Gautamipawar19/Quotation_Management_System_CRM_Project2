import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { createQuote } from "../api/quoteApi";
import QuoteForm from "../components/quotes/QuoteForm";
import ErrorMessage from "../components/common/ErrorMessage";

export default function CreateQuotePage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleCreateQuote = async (payload) => {
    try {
      setLoading(true);
      setError("");

      await createQuote(payload);
      navigate("/quotes");
    } catch (err) {
      console.log("Create Quote Error:", err?.response?.data || err);

      const responseData = err?.response?.data;
      const validationErrors = responseData?.errors;

      if (validationErrors) {
        const flatErrors = Object.values(validationErrors).flat().join(" ");
        setError(flatErrors);
      } else {
        setError(
          responseData?.message ||
            responseData?.title ||
            "Failed to create quote."
        );
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h1>Create Quote</h1>
      <ErrorMessage message={error} />
      <QuoteForm onSubmit={handleCreateQuote} loading={loading} />
    </div>
  );
}