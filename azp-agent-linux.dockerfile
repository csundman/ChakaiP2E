FROM alpine

ENV TARGETARCH="linux-musl-x64"

ENV PATH="/venv/bin:$PATH"

# Ref: https://rokpoto.com/install-azure-cli-dockerfile-alpine/
RUN set -ex \
 && apk update \
 && apk upgrade \
 && apk add --no-cache --update \
    bash \
    curl \
    git \
    icu-libs \
    nodejs \
    npm \
    jq \
    python3 \
    py3-pip \
 && apk add --no-cache --update --virtual=build \
    gcc \
    musl-dev \
    python3-dev \
    libffi-dev \
    linux-headers \
    openssl-dev \
    cargo \
    make \
 && python3 -m venv /venv \
 && pip3 install --no-cache-dir --prefer-binary \
    azure-cli \
 && apk del build

WORKDIR /azp/

COPY ./start.sh ./

RUN chmod +x ./start.sh

RUN adduser -D agent

RUN chown agent ./

USER agent

# Another option is to run the agent as root.
# ENV AGENT_ALLOW_RUNASROOT="true"

ENTRYPOINT [ "./start.sh" ]