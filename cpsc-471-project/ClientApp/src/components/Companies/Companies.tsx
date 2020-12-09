import { CircularProgress } from '@material-ui/core';
import axios from 'axios';
import React, { useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import { ICompany } from '../../models/ICompany';
import './Companies.css';

const CompanyCard = ({ company }: { company: ICompany }) => {
  return (
    <Link to={`/companies/${company.companyId}`}>
      <div className="company-card">
        <p>{company.name}</p>
      </div>
    </Link>
  );
};

const Companies = () => {
  const { getHeaders } = useContext(AuthContext);
  const [companies, setCompanies] = useState<ICompany[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (loading) setLoading(true);
    axios
      .get('/api/companies', getHeaders())
      .then((res) => {
        console.log(res.data);
        setCompanies(res.data);
      })
      .catch((err) => console.log(err))
      .finally(() => setLoading(false));
  }, [loading, getHeaders]);

  return (
    <div className="companies-container">
      {loading ? (
        <CircularProgress />
      ) : (
        <>
          {companies.map((company) => (
            <CompanyCard key={company.companyId} company={company} />
          ))}
        </>
      )}
    </div>
  );
};

export default Companies;
