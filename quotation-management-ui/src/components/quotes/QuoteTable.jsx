import { Link } from "react-router-dom";
import QuoteStatusBadge from "./QuoteStatusBadge";

export default function QuoteTable({ quotes }) {
  if (!quotes || quotes.length === 0) {
    return <p>No quotes found.</p>;
  }

  return (
    <table className="quotes-table">
      <thead>
        <tr>
          <th>Quote No</th>
          <th>Lead ID</th>
          <th>Customer ID</th>
          <th>Status</th>
          <th>Total</th>
          <th>Action</th>
        </tr>
      </thead>
      <tbody>
        {quotes.map((quote) => (
          <tr key={quote.quoteId}>
            <td>{quote.quoteNumber}</td>
            <td>{quote.leadId ?? "N/A"}</td>
            <td>{quote.customerId ?? "N/A"}</td>
            <td>
              <QuoteStatusBadge status={quote.status} />
            </td>
            <td>{quote.grandTotal}</td>
            <td>
              <Link to={`/quotes/${quote.quoteId}`}>View</Link>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}