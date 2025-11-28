import React from 'react';
import { Pagination as BSPagination } from 'react-bootstrap';
import { PaginationMetadata } from '@/types';

interface PaginationProps {
  metadata: PaginationMetadata;
  onPageChange: (page: number) => void;
}

const Pagination: React.FC<PaginationProps> = ({ metadata, onPageChange }) => {
  const { currentPage, totalPages, hasPrevious, hasNext } = metadata;

  const getPageNumbers = () => {
    const pages: (number | string)[] = [];
    const maxVisible = 5;

    if (totalPages <= maxVisible) {
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      if (currentPage <= 3) {
        for (let i = 1; i <= 4; i++) {
          pages.push(i);
        }
        pages.push('...');
        pages.push(totalPages);
      } else if (currentPage >= totalPages - 2) {
        pages.push(1);
        pages.push('...');
        for (let i = totalPages - 3; i <= totalPages; i++) {
          pages.push(i);
        }
      } else {
        pages.push(1);
        pages.push('...');
        for (let i = currentPage - 1; i <= currentPage + 1; i++) {
          pages.push(i);
        }
        pages.push('...');
        pages.push(totalPages);
      }
    }

    return pages;
  };

  if (totalPages <= 1) {
    return null;
  }

  return (
    <div className="d-flex justify-content-between align-items-center mt-3">
      <div className="text-muted">
        Page {currentPage} of {totalPages}
      </div>
      <BSPagination className="mb-0">
        <BSPagination.First
          disabled={!hasPrevious}
          onClick={() => onPageChange(1)}
        />
        <BSPagination.Prev
          disabled={!hasPrevious}
          onClick={() => onPageChange(currentPage - 1)}
        />
        
        {getPageNumbers().map((page, index) => {
          if (page === '...') {
            return <BSPagination.Ellipsis key={`ellipsis-${index}`} disabled />;
          }
          return (
            <BSPagination.Item
              key={page}
              active={page === currentPage}
              onClick={() => onPageChange(page as number)}
            >
              {page}
            </BSPagination.Item>
          );
        })}
        
        <BSPagination.Next
          disabled={!hasNext}
          onClick={() => onPageChange(currentPage + 1)}
        />
        <BSPagination.Last
          disabled={!hasNext}
          onClick={() => onPageChange(totalPages)}
        />
      </BSPagination>
    </div>
  );
};

export default Pagination;
