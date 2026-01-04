import {
  CContainer,
  CAccordion,
  CAccordionBody,
  CAccordionHeader,
  CAccordionItem,
} from "@coreui/react";

import "./StudentSupportPage.css";
import CategoryCard from "./CategoryCard";
import SupportRequestForm from "./SupportRequestForm";
import { useEffect, useState } from "react";

import { getFaqs } from "../../service/student-support/api";
import type { FAQ } from "../../service/student-support/types";
import { useAPI } from "../../context/services";


export default function StudentSupportPage() {
  const api = useAPI();

  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);
  const [faqs, setFaqs] = useState<FAQ[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getFaqs(api)
      .then((data) => {
        setFaqs(data);
        setLoading(false);
      })
      .catch((err: any) => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  return (
    <div className="ss-page">
      <main className="ss-main">
        <CContainer fluid className="ss-mainInner">
          <header className="ss-top">
            <h1 className="ss-title">Welcome back, Jane!</h1>
            <p className="ss-subtitle">
              Find answers or submit a request for assistance
            </p>
          </header>

          <section className="ss-content">
            <div className="ss-twoCol">
              <div className="ss-categories">
                <CategoryCard
                  title="Academic support"
                  description="Questions about exams, grades or enrolment ..."
                  onClick={() => setSelectedCategoryId(1)}
                />
                <CategoryCard
                  title="Technical issues"
                  description="Platform errors, UI errors, login ..."
                  onClick={() => setSelectedCategoryId(2)}
                />
                <CategoryCard
                  title="Administrative help"
                  description="Payments, documentation ..."
                  onClick={() => setSelectedCategoryId(3)}
                />
                <CategoryCard
                  title="Account & Security"
                  description="Profile security update"
                  onClick={() => setSelectedCategoryId(4)}
                />
              </div>

              <div className="ss-formSpace">
                <SupportRequestForm selectedCategoryId={selectedCategoryId} />
              </div>
            </div>

            <div className="ss-belowEmpty">
              <section className="ss-faq">
                <h3 className="ss-faqTitle">Frequently Asked Questions</h3>

                {loading && <p>Loading FAQs...</p>}
                {error && <p>{error}</p>}

                {!loading && !error && (
                  <CAccordion>
                    {faqs.map((faq) => (
                      <CAccordionItem key={faq.id} itemKey={faq.id}>
                        <CAccordionHeader>
                          {faq.question}
                        </CAccordionHeader>
                        <CAccordionBody>
                          {faq.answer}
                        </CAccordionBody>
                      </CAccordionItem>
                    ))}
                  </CAccordion>
                )}
              </section>

            </div>
          </section>

        </CContainer>
      </main>
    </div>
  );
}
