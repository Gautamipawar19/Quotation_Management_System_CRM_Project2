import { useState } from "react";

const defaultLineItem = {
  itemDescription: "",
  quantity: 1,
  unitPrice: 0,
  discount: 0,
};

const today = new Date().toISOString().split("T")[0];
const after7Days = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000)
  .toISOString()
  .split("T")[0];

export default function QuoteForm({ onSubmit, initialData = null, loading = false }) {
  const [formData, setFormData] = useState(
    initialData || {
      leadId: "",
      customerId: "",
      quoteDate: today,
      expiryDate: after7Days,
      taxAmount: 0,
      lineItems: [defaultLineItem],
    }
  );

  const [error, setError] = useState("");

  const handleLineItemChange = (index, field, value) => {
    const updatedItems = [...formData.lineItems];
    updatedItems[index] = {
      ...updatedItems[index],
      [field]: field === "itemDescription" ? value : Number(value),
    };

    setFormData((prev) => ({
      ...prev,
      lineItems: updatedItems,
    }));
  };

  const addLineItem = () => {
    setFormData((prev) => ({
      ...prev,
      lineItems: [...prev.lineItems, { ...defaultLineItem }],
    }));
  };

  const removeLineItem = (index) => {
    if (formData.lineItems.length === 1) return;

    const updatedItems = formData.lineItems.filter((_, i) => i !== index);

    setFormData((prev) => ({
      ...prev,
      lineItems: updatedItems,
    }));
  };

  const calculateSubtotal = () => {
    return formData.lineItems.reduce((sum, item) => {
      const lineTotal =
        Number(item.quantity) * Number(item.unitPrice) - Number(item.discount || 0);
      return sum + lineTotal;
    }, 0);
  };

  const grandTotal = calculateSubtotal() + Number(formData.taxAmount || 0);

  const handleSubmit = (e) => {
    e.preventDefault();
    setError("");

    if (!formData.leadId && !formData.customerId) {
      setError("Please enter either Lead ID or Customer ID.");
      return;
    }

    if (formData.leadId && formData.customerId) {
      setError("Please enter only one: Lead ID or Customer ID.");
      return;
    }

    if (!formData.quoteDate) {
      setError("Quote Date is required.");
      return;
    }

    if (!formData.expiryDate) {
      setError("Expiry Date is required.");
      return;
    }

    if (new Date(formData.expiryDate) <= new Date(formData.quoteDate)) {
      setError("Expiry Date must be greater than Quote Date.");
      return;
    }

    if (formData.lineItems.length === 0) {
      setError("At least one line item is required.");
      return;
    }

    for (const item of formData.lineItems) {
      if (!item.itemDescription?.trim()) {
        setError("Item description is required.");
        return;
      }

      if (Number(item.quantity) <= 0) {
        setError("Quantity must be greater than 0.");
        return;
      }

      if (Number(item.unitPrice) <= 0) {
        setError("Unit price must be greater than 0.");
        return;
      }

      if (Number(item.discount) < 0) {
        setError("Discount cannot be negative.");
        return;
      }
    }

    const savedUser = JSON.parse(localStorage.getItem("user") || "null");
    const username = savedUser?.username || "sakshi";

    const payload = {
      leadId: formData.leadId ? Number(formData.leadId) : null,
      customerId: formData.customerId ? Number(formData.customerId) : null,
      quoteDate: formData.quoteDate,
      expiryDate: formData.expiryDate,
      taxAmount: Number(formData.taxAmount),
      createdBy: username,
      lineItems: formData.lineItems.map((item) => ({
        itemDescription: item.itemDescription.trim(),
        quantity: Number(item.quantity),
        unitPrice: Number(item.unitPrice),
        discount: Number(item.discount),
      })),
    };

    console.log("Create Quote Payload:", payload);
    onSubmit(payload);
  };

  return (
    <form onSubmit={handleSubmit} className="quote-form">
      <div className="form-group">
        <label>Lead ID</label>
        <input
          type="number"
          value={formData.leadId}
          onChange={(e) =>
            setFormData((prev) => ({ ...prev, leadId: e.target.value }))
          }
        />
      </div>

      <div className="form-group">
        <label>Customer ID</label>
        <input
          type="number"
          value={formData.customerId}
          onChange={(e) =>
            setFormData((prev) => ({ ...prev, customerId: e.target.value }))
          }
        />
      </div>

      <div className="form-group">
        <label>Quote Date</label>
        <input
          type="date"
          value={formData.quoteDate}
          onChange={(e) =>
            setFormData((prev) => ({ ...prev, quoteDate: e.target.value }))
          }
        />
      </div>

      <div className="form-group">
        <label>Expiry Date</label>
        <input
          type="date"
          value={formData.expiryDate}
          onChange={(e) =>
            setFormData((prev) => ({ ...prev, expiryDate: e.target.value }))
          }
        />
      </div>

      <h3>Line Items</h3>

      {formData.lineItems.map((item, index) => (
        <div key={index} className="line-item-card">
          <input
            type="text"
            placeholder="Item description"
            value={item.itemDescription}
            onChange={(e) =>
              handleLineItemChange(index, "itemDescription", e.target.value)
            }
          />
          <input
            type="number"
            placeholder="Quantity"
            value={item.quantity}
            onChange={(e) =>
              handleLineItemChange(index, "quantity", e.target.value)
            }
          />
          <input
            type="number"
            placeholder="Unit Price"
            value={item.unitPrice}
            onChange={(e) =>
              handleLineItemChange(index, "unitPrice", e.target.value)
            }
          />
          <input
            type="number"
            placeholder="Discount"
            value={item.discount}
            onChange={(e) =>
              handleLineItemChange(index, "discount", e.target.value)
            }
          />
          <button type="button" onClick={() => removeLineItem(index)}>
            Remove
          </button>
        </div>
      ))}

      <button type="button" onClick={addLineItem}>
        Add Line Item
      </button>

      <div className="form-group">
        <label>Tax Amount</label>
        <input
          type="number"
          value={formData.taxAmount}
          onChange={(e) =>
            setFormData((prev) => ({
              ...prev,
              taxAmount: e.target.value,
            }))
          }
        />
      </div>

      <div className="totals-box">
        <p>Subtotal: {calculateSubtotal()}</p>
        <p>Grand Total: {grandTotal}</p>
      </div>

      {error && <p className="error-message">{error}</p>}

      <button type="submit" disabled={loading}>
        {loading ? "Saving..." : "Save Quote"}
      </button>
    </form>
  );
}