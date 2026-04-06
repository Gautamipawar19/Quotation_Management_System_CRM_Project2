import { Link } from "react-router-dom";

export default function Sidebar() {
  return (
    <aside className="sidebar">
      <h3>Menu</h3>
      <nav>
        <Link to="/dashboard">Dashboard</Link>
        <Link to="/quotes">Quotes</Link>
        <Link to="/quotes/create">Create Quote</Link>
      </nav>
    </aside>
  );
}