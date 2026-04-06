import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { getQuoteById } from "../api/quoteApi";
import Loader from "../components/common/Loader";
import ErrorMessage from "../components/common/ErrorMessage";

export default function QuoteDetailsPage() {
  const { id } = useParams();
  const [quote, setQuote] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchQuote();
  }, [id]);

  const fetchQuote = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await getQuoteById(id);
      console.log("Quote Details API Response:", response);

      const quoteData = response?.data || response;
      setQuote(quoteData);
    } catch (err) {
      console.log("Quote Details Error:", err?.response?.data || err);
      setError(
        err?.response?.data?.message || "Failed to load quote details."
      );
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <Loader />;
  if (error) return <ErrorMessage message={error} />;
  if (!quote) return <p>Quote not found.</p>;

  const quoteId = quote.quoteId ?? quote.id;
  const quoteNumber = quote.quoteNumber ?? "N/A";
  const status = quote.status ?? "N/A";
  const leadId = quote.leadId ?? "N/A";
  const customerId = quote.customerId ?? "N/A";
  const createdBy = quote.createdBy ?? "N/A";
  const createdDate = quote.createdDate
    ? new Date(quote.createdDate).toLocaleString()
    : "N/A";
  const quoteDate = quote.quoteDate
    ? new Date(quote.quoteDate).toLocaleDateString()
    : "N/A";
  const expiryDate = quote.expiryDate
    ? new Date(quote.expiryDate).toLocaleDateString()
    : "N/A";
  const subTotal = quote.subTotal ?? 0;
  const taxAmount = quote.taxAmount ?? 0;
  const discountAmount = quote.discountAmount ?? 0;
  const grandTotal = quote.grandTotal ?? 0;
  const lineItems = quote.lineItems ?? [];

  return (
    <div>
      <h1>Quote Details</h1>

      <div className="quote-details-card">
        <p><strong>Quote Number:</strong> {quoteNumber}</p>
        <p><strong>Status:</strong> {status}</p>
        <p><strong>Lead ID:</strong> {leadId}</p>
        <p><strong>Customer ID:</strong> {customerId}</p>
        <p><strong>Created By:</strong> {createdBy}</p>
        <p><strong>Created Date:</strong> {createdDate}</p>
        <p><strong>Quote Date:</strong> {quoteDate}</p>
        <p><strong>Expiry Date:</strong> {expiryDate}</p>
        <p><strong>Subtotal:</strong> {subTotal}</p>
        <p><strong>Tax Amount:</strong> {taxAmount}</p>
        <p><strong>Discount Amount:</strong> {discountAmount}</p>
        <p><strong>Grand Total:</strong> {grandTotal}</p>
      </div>

      <h3>Line Items</h3>

      {lineItems.length > 0 ? (
        <table className="quotes-table">
          <thead>
            <tr>
              <th>Description</th>
              <th>Quantity</th>
              <th>Unit Price</th>
              <th>Discount</th>
              <th>Line Total</th>
            </tr>
          </thead>
          <tbody>
            {lineItems.map((item, index) => (
              <tr key={item.lineItemId ?? index}>
                <td>{item.itemDescription}</td>
                <td>{item.quantity}</td>
                <td>{item.unitPrice}</td>
                <td>{item.discount}</td>
                <td>{item.lineTotal ?? item.quantity * item.unitPrice - item.discount}</td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p>No line items found.</p>
      )}

      {status === "Draft" && quoteId && (
        <Link to={`/quotes/${quoteId}/edit`} className="primary-btn">
          Edit Quote
        </Link>
      )}
    </div>
  );
}