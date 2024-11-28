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