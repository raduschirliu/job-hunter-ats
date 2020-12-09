import { CircularProgress } from '@material-ui/core';
import React, { useContext, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import ICompany from '../../models/ICompany';
import axios from 'axios';
import './Company.css';

const Company = () => {
  const { getHeaders } = useContext(AuthContext);
  const { companyId } = useParams<any>();
  const [company, setCompany] = useState<ICompany | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (loading) return;
    setLoading(true);

    axios
      .get(`/api/companies/${companyId}`, getHeaders())
      .then((res) => {
        setCompany(res.data);
      })
      .catch(err => console.log(err))
      .finally(() => setLoading(false));
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [companyId, getHeaders]);

  if (loading) {
    return (
      <div className="company-container">
        <CircularProgress />
      </div>
    );
  }

  return (
    <div className="company-container">
      {company ? (
        <>
          <div className="company-segment">
            <p className="company-title">Name</p>
            <p>{company.name}</p>
          </div>
          <div className="company-segment">
            <p className="company-title">Industry</p>
            <p>{company.industry}</p>
          </div>
          <div className="company-segment">
            <p className="company-title">Description</p>
            <p>{company.description}</p>
          </div>
        </>
      ) : (
        <p>Company does not exist :(</p>
      )}
    </div>
  );
};

export default Company;
