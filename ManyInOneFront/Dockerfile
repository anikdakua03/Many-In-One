# FROM node:20.5.0-alpine AS build
# WORKDIR /app
# COPY package*.json ./

# RUN npm install
# # this command executes ngcc and provided the compiler options using ngx 
# RUN npx ngcc --properties es2023 browser module main --first-only --create-ivy-entry-points

# COPY . .

# RUN npm run build

# # Next stage
# FROM nginx:stable
# # this is taht setting to try getting our index.html configuaration
# COPY default.conf /etc/nginx/conf.d/default.conf
# # Copies from build and places it to nginx/html folder
# COPY --from=build /app/dist/many-in-one-front /usr/share/nginx/html
# EXPOSE 80

# trying this
FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build
# removing extra node module s, no more needed
# RUN rm -rf node_modules
# Stage 2: Serve the angular app with Nginx
FROM nginx:latest
WORKDIR /usr/share/nginx/html
# when ng build can see it is under browser also , so ..
# this was for without SSR
COPY --from=build /app/dist/many-in-one-front/browser /app/dist/many-in-one-front/server /usr/share/nginx/html/
# this is for with SSR, so we need both browser and server folder
# COPY --from=build /app/dist/many-in-one-front /usr/share/nginx/html
COPY default.conf /etc/nginx/conf.d/default.conf
EXPOSE 80