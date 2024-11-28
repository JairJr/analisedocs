import streamlit as st
from services.blob_service import upload_blob
from services.credit_card_service import analyze_credit_card

def configure_interface():
    st.title("Upload de arquivo DIO - Desafio 1 - Azure")
    uploaded_file = st.file_uploader("escolha um arquivo", type=["png", "jpg", "jpeg"])

    if uploaded_file is not None:
        st.image(uploaded_file, caption="Arquivo enviado")
        fileName = uploaded_file.name
        #enviar para o azure
        blob_url = upload_blob(uploaded_file, fileName)
        if blob_url is not None:
            st.write(f"Arquivo enviado com sucesso para o Azure Blob Storage: {blob_url}")
            credit_card_info = analyze_credit_card(blob_url)
            show_image_and_validation(blob_url, credit_card_info)
        else:
            st.write("Erro ao enviar arquivo para o Azure Blob Storage") 

def show_image_and_validation(blob_url, credit_card_info):
    st.image(blob_url, caption="Arquivo enviado")    
    st.write("resultado da validação:")
    st.write(credit_card_info)
    if credit_card_info and credit_card_info["card_name"]:
        st.markdown(f"<h1 style='color: green;'>Cartão válido {credit_card_info['card_name']}</h1>", unsafe_allow_html=True)
        st.write("nome do titular: ", credit_card_info["card_name"])
        st.write("nome do titular: ", credit_card_info["card_number"])
    else:
        st.markdown(f"<h1 style='color: red;'>Cartão inválido</h1>", unsafe_allow_html=True)
        st.write("cartão inválido")
        st.write("nome do titular: ", credit_card_info["card_name"])
        st.write("nome do titular: ", credit_card_info["card_number"])

if __name__ == "__main__":
    configure_interface()