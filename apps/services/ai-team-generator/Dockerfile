FROM python:3.13-slim


WORKDIR /app

RUN apt-get update && apt-get install -y \
    curl \
    gnupg \
    build-essential


RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt


COPY . .

CMD ["uvicorn", "main2:app", "--host", "0.0.0.0", "--port", "8001"]
