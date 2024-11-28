from azure.core.credentials import AzureKeyCredential
from azure.ai.documentintelligence import DocumentIntelligenceClient
from azure.ai.documentintelligence.models import AnalyzeDocumentRequest
from utils.config import Config


def analyze_credit_card(card_url):
    # try:
    credentials = AzureKeyCredential(Config.KEY)

    doc_intel_client = DocumentIntelligenceClient(Config.ENDPOINT, credentials)

    # card_info = doc_intel_client.begin_analyze_document("prebuilt-creditCard", card_url).result()
    card_info = doc_intel_client.begin_analyze_document("prebuilt-creditCard", AnalyzeDocumentRequest(url_source=card_url))        
    result = card_info.result()

    for document in result.documents:
        fields = document.get('fields',{})

        return {
            "card_name": fields.get('CardHolderName',{}).get('content'),
            "card_number": fields.get('CardNumber',{}).get('content'),
            "expire_date": fields.get('ExpirationDate',{}).get('content'),
            "bank_name": fields.get('IssuingBank',{}).get('content'),
        }



import os
import streamlit as st
from azure.storage.blob import BlobServiceClient
from utils.config import Config

def upload_blob(file, file_name):
    try:
        blob_service_client = BlobServiceClient.from_connection_string(Config.AZURE_STORAGE_CONNECTION_STRING)
        blob_client = blob_service_client.get_blob_client(container=Config.AZURE_STORAGE_CONTAINER_NAME, blob=file_name)
        #with open(file, "rb") as data:
        blob_client.upload_blob(file, overwrite=True)
        return blob_client.url
    except Exception as e:
        st.write(f"Erro ao enviar arquivo para o Azure Blob Storage: {e}")
        return None