import { useEffect, useState } from "react";
import { getAllQuotes } from "../api/quoteApi";
import Loader from "../components/common/Loader";
import ErrorMessage from "../components/common/ErrorMessage";

export default function DashboardPage() {
  const [quotes, setQuotes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await getAllQuotes();
      console.log("Dashboard Quotes API Response:", response);

      const quotesArray = response?.data || [];
      setQuotes(quotesArray);
    } catch (err) {
      console.log("Dashboard Error:", err?.response?.data || err);
      setError("Failed to load dashboard data.");
    } finally {
      setLoading(false);
    }
  };

  const getStatus = (q) => String(q.status || "").toLowerCase();

  const totalQuotes = quotes.length;
  const draftQuotes = quotes.filter((q) => getStatus(q) === "draft").length;
  const approvedQuotes = quotes.filter(
    (q) => getStatus(q) === "accepted" || getStatus(q) === "approved"
  ).length;
  const rejectedQuotes = quotes.filter((q) => getStatus(q) === "rejected").length;

  if (loading) return <Loader />;

  return (
    <div>
      <h1>Dashboard</h1>
      <ErrorMessage message={error} />

      <div className="dashboard-cards">
        <div className="card">
          <h3>Total Quotes</h3>
          <p>{totalQuotes}</p>
        </div>

        <div className="card">
          <h3>Draft Quotes</h3>
          <p>{draftQuotes}</p>
        </div>

        <div className="card">
          <h3>Approved Quotes</h3>
          <p>{approvedQuotes}</p>
        </div>

        <div className="card">
          <h3>Rejected Quotes</h3>
          <p>{rejectedQuotes}</p>
        </div>
      </div>
    </div>
  );
}