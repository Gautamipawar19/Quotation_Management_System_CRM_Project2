import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getQuoteById, updateQuote } from "../api/quoteApi";
import QuoteForm from "../components/quotes/QuoteForm";
import Loader from "../components/common/Loader";

export default function EditQuotePage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [quote, setQuote] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchQuote();
  }, [id]);

  const fetchQuote = async () => {
    try {
      setLoading(true);
      const data = await getQuoteById(id);
      setQuote(data);
    } catch (err) {
      alert(err?.response?.data?.message || "Failed to load quote.");
    } finally {
      setLoading(false);
    }
  };

  const handleUpdate = async (payload) => {
    try {
      setSaving(true);
      await updateQuote(id, payload);
      navigate(`/quotes/${id}`);
    } catch (err) {
      alert(err?.response?.data?.message || "Failed to update quote.");
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <Loader />;
  if (!quote) return <p>Quote not found.</p>;

  return (
    <div>
      <h1>Edit Quote</h1>
      <QuoteForm initialData={quote} onSubmit={handleUpdate} loading={saving} />
    </div>
  );
}