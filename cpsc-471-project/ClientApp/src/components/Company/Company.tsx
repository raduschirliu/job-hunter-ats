import { CircularProgress } from '@material-ui/core';
import React, { useContext, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import { ICompany } from '../../models/Company';
import axios from 'axios';
import './Company.css';

const Company = () => {
  const { getHeaders } = useContext(AuthContext);
  const { companyId } = useParams<any>();
  const [company, setCompany] = useState<ICompany | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    axios
      .get(`/api/companies/${companyId}`, getHeaders())
      .then((res) => {
        setCompany(res.data);
      })
      .catch(err => console.log(err))
      .finally(() => setLoading(false));
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
          <p>{company.name}</p>
          <p>{company.industry}</p>
          <p>{company.description}</p>
        </>
      ) : (
        <p>Company does not exist :(</p>
      )}
    </div>
  );
};

export default Company;
