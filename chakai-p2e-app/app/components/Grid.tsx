"use client";

import { useState } from 'react';

interface Column<T> {
  field: keyof T;
  header: string;
}

interface GridProps<T> {
  data: T[];
  columns: Column<T>[];
  onRowClick?: (item: T) => void;
}

type SortDirection = 'asc' | 'desc' | null;

export function Grid<T>({ data, columns: initialColumns, onRowClick }: GridProps<T>) {
  const [columns, setColumns] = useState<Column<T>[]>(initialColumns);
  const [sortField, setSortField] = useState<keyof T | null>(null);
  const [sortDirection, setSortDirection] = useState<SortDirection>(null);
  const [draggedColumn, setDraggedColumn] = useState<number | null>(null);

  const handleSort = (field: keyof T) => {
    if (sortField === field) {
      if (sortDirection === 'asc') setSortDirection('desc');
      else if (sortDirection === 'desc') {
        setSortDirection(null);
        setSortField(null);
      }
    } else {
      setSortField(field);
      setSortDirection('asc');
    }
  };

  const handleDragStart = (index: number) => {
    setDraggedColumn(index);
  };

  const handleDragOver = (e: React.DragEvent, index: number) => {
    e.preventDefault();
  };

  const handleDrop = (e: React.DragEvent, dropIndex: number) => {
    e.preventDefault();
    if (draggedColumn === null) return;
    
    const newColumns = [...columns];
    const [draggedItem] = newColumns.splice(draggedColumn, 1);
    newColumns.splice(dropIndex, 0, draggedItem);
    
    setColumns(newColumns);
    setDraggedColumn(null);
  };

  const getSortedData = () => {
    if (!sortField || !sortDirection) return data;

    return [...data].sort((a, b) => {
      const aValue = a[sortField];
      const bValue = b[sortField];

      if (aValue === bValue) return 0;
      
      const comparison = aValue < bValue ? -1 : 1;
      return sortDirection === 'asc' ? comparison : -comparison;
    });
  };

  const getSortIcon = (field: keyof T) => {
    if (sortField !== field) return '⇅';
    return sortDirection === 'asc' ? '↑' : '↓';
  };

  return (
    <table className="min-w-full border-collapse">
      <thead>
        <tr className="bg-gray-100 dark:bg-gray-800">
          {columns.map((column, index) => (
            <th 
              key={String(column.field)}
              draggable
              onDragStart={() => handleDragStart(index)}
              onDragOver={(e) => handleDragOver(e, index)}
              onDrop={(e) => handleDrop(e, index)}
              className={`border-b border-gray-200 dark:border-gray-700 px-4 py-2 text-left 
                text-gray-900 dark:text-gray-100 cursor-pointer hover:bg-gray-200 dark:hover:bg-gray-700
                ${draggedColumn === index ? 'opacity-50' : ''}`}
              onClick={() => handleSort(column.field)}
            >
              <div className="flex items-center gap-2">
                <span className="text-gray-400 dark:text-gray-500">⋮⋮</span>
                {column.header}
                <span className={`text-sm ${
                  sortField === column.field 
                    ? 'text-gray-700 dark:text-gray-300' 
                    : 'text-gray-400 dark:text-gray-600'
                }`}>
                  {getSortIcon(column.field)}
                </span>
              </div>
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {getSortedData().map((item) => (
          <tr 
            key={String(item[columns[0].field])}
            onClick={() => onRowClick?.(item)}
            className={`border-b border-gray-200 dark:border-gray-700 text-gray-900 dark:text-gray-100
              ${onRowClick ? "cursor-pointer hover:bg-gray-50 dark:hover:bg-gray-800" : ""}`}
          >
            {columns.map((column) => (
              <td key={String(column.field)} className="px-4 py-2">
                {String(item[column.field])}
              </td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
}