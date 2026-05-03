from django.urls import path
from . import views

urlpatterns = [
    path('run/config/', views.run_config),
    path('battle/monster-move/', views.monster_move),
]