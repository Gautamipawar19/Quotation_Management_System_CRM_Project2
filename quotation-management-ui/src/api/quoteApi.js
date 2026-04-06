import api from "./axios";

export const getAllQuotes = async (pageNumber = 1, pageSize = 1000) => {
  const response = await api.get(
    `/quotes?pageNumber=${pageNumber}&pageSize=${pageSize}`
  );
  return response.data;
};

export const getQuoteById = async (id) => {
  const response = await api.get(`/quotes/${id}`);
  return response.data;
};

export const createQuote = async (data) => {
  const response = await api.post("/quotes", data);
  return response.data;
};

export const updateQuote = async (id, data) => {
  const response = await api.put(`/quotes/${id}`, data);
  return response.data;
};