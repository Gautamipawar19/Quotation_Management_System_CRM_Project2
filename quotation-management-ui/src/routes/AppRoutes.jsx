import { Routes, Route } from "react-router-dom";
import LoginPage from "../pages/LoginPage";
import DashboardPage from "../pages/DashboardPage";
import QuotesPage from "../pages/QuotesPage";
import CreateQuotePage from "../pages/CreateQuotePage";
import QuoteDetailsPage from "../pages/QuoteDetailsPage";
import EditQuotePage from "../pages/EditQuotePage";
import UnauthorizedPage from "../pages/UnauthorizedPage";
import NotFoundPage from "../pages/NotFoundPage";
import ProtectedRoute from "../components/common/ProtectedRoute";
import AppLayout from "../components/layout/AppLayout";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<LoginPage />} />
      <Route path="/unauthorized" element={<UnauthorizedPage />} />

      <Route
        element={
          <ProtectedRoute>
            <AppLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/quotes" element={<QuotesPage />} />
        <Route path="/quotes/create" element={<CreateQuotePage />} />
        <Route path="/quotes/:id" element={<QuoteDetailsPage />} />
        <Route path="/quotes/:id/edit" element={<EditQuotePage />} />
      </Route>

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}